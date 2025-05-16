import asyncio

import json
import os

from langchain_anthropic import ChatAnthropic
from dotenv import load_dotenv
from mcp_use import MCPClient, MCPAgent


async def ask_ia(prompt):
    load_dotenv()

    config = {
        "mcpServers": {
            "postgres": {
                "command": "npx",
                "args": [
                    "-y",
                    "@modelcontextprotocol/server-postgres",
                    "postgresql://postgres:postgres@postgres:5432/postgres-db",
                ],
            }
        }
    }
 

    client = MCPClient.from_dict(config)

    llm = ChatAnthropic(
        model="claude-3-5-sonnet-latest",
        anthropic_api_key=os.getenv("CLAUDE_API_KEY") 
    )

    agent = MCPAgent(llm=llm, client=client, max_steps=30)

    try:
        result = await agent.run(
            f"{prompt} Devuelve los resultados exclusivamente en formato JSON válido, sin texto adicional.",
            max_steps=30,
        )
        if not result:
            print("La consulta no devolvió resultados")
            return None

        try:
            if isinstance(result, str):
                return json.loads(result)
            return result
        except json.JSONDecodeError as e:
            print(f"Error decodificando JSON: {e}")
            return None

    except Exception as e:
        print(f"An error occurred: {e}")
        return None
    finally:
        if client.sessions:
            await client.close_all_sessions()


if __name__ == "__main__":
    asyncio.run(ask_ia())
