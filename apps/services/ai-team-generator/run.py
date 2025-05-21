import asyncio
from team_db_service import TeamDatabaseService

async def main():
    connection_string = "postgresql://postgres:postgres@localhost:5432/postgres-db"
    service = TeamDatabaseService(connection_string)

    await service.connect()

    # Simulación del request entrante
    request_data = {
        "CreatorId": "2d407712-c8f4-4d8f-9b93-3f6e2d1b91b9",
        "TeamSize": 3,
        "Requirements": [
            { "Role": "Developer", "Area": "Web Development", "Level": "Junior" },
            { "Role": "Developer", "Area": "Web Development", "Level": "Junior" },
            { "Role": "Developer", "Area": "Web Development", "Level": "Junior" }
        ],
        "Technologies": ["C#"],
        "SfiaLevel": 3,
        "Weights": {
            "SfiaWeight": 20,
            "TechnicalWeight": 25,
            "PsychologicalWeight": 15,
            "ExperienceWeight": 15,
            "LanguageWeight": 10,
            "InterestsWeight": 5,
            "TimezoneWeight": 10
        },
        "Availability": True
    }

    candidates = await service.get_generation_candidates(
        requirements=request_data["Requirements"],
        technologies=request_data["Technologies"],
        min_sfia_level=request_data["SfiaLevel"],
        availability=request_data["Availability"]
    )

    print("\n=== Generation Candidates ===")
    for candidate in candidates:
        print(candidate)

    await service.disconnect()

if __name__ == "__main__":
    asyncio.run(main())
