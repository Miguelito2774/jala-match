# services/ai-team-generator/main.py
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import os
import json
from dotenv import load_dotenv
from anthropic import Anthropic

app = FastAPI()

load_dotenv()

class TeamGenerationRequest(BaseModel):
    roles: list[str]
    technologies: list[str]
    sfia_level: int
    availability: bool
    members_data: list[dict]
    criteria_weights: dict

class TeamMemberCompatibilityRequest(BaseModel):
    team: dict
    new_member: dict

@app.post("/generate-teams")
async def generate_teams(request: TeamGenerationRequest):
    try:
        client = Anthropic(api_key=os.getenv('CLAUDE_API_KEY'))
        
        prompt = f"""
        Analyze the following team member data and generate optimized teams based on comprehensive compatibility analysis.

        Generate teams based on the following requirements:
        - Roles: {request.roles}
        - Technologies: {request.technologies}
        - SFIA Level: {request.sfia_level}
        - Availability: {request.availability}

        Evaluation Criteria and Weights:
        1. Technical Compatibility (tech_stack): {request.criteria_weights['technical']}%
        2. Psychological Compatibility (MBTI): {request.criteria_weights['psychological']}%
        3. Interest-based Compatibility: {request.criteria_weights['interests']}%

        Team Formation Requirements:
        - Every team MUST have at least 5 members.
        - Maximize overall team compatibility while ensuring individual fit
        - Consider potential conflicts
        - Aim for diverse skill sets while maintaining strong compatibility scores

        Team Member Data:
        {json.dumps(request.members_data, indent=2, ensure_ascii=False)}

        Respond ONLY with a JSON object using the specified structure.
        """
        
        response = client.messages.create(
            model="claude-3-sonnet-20240229",
            max_tokens=2000,
            temperature=0.2,
            messages=[{"role": "user", "content": prompt}]
        )
        
        return json.loads(response.content[0].text)
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/calculate-compatibility")
async def calculate_compatibility(request: TeamMemberCompatibilityRequest):
    try:
        client = Anthropic(api_key=os.getenv('CLAUDE_API_KEY'))
        
        prompt = f"""
        Analyze the compatibility of the following new member with the existing team:

        Existing Team Members:
        {json.dumps(request.team['members'], indent=2, ensure_ascii=False)}

        New Member:
        {json.dumps(request.new_member, indent=2, ensure_ascii=False)}

        Respond ONLY with a JSON object containing compatibility score and justification.
        """
        
        response = client.messages.create(
            model="claude-3-5-sonnet-20241022",
            max_tokens=1000,
            messages=[{"role": "user", "content": prompt}]
        )
        
        return json.loads(response.content[0].text)
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))