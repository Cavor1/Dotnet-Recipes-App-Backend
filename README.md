## Recipes App Backend

Backend application exposing a REST API for managing recipes, meals, ingredients, and calorie tracking.

The app can be run locally using Docker, which starts the API and database in a reproducible development environment. The project also includes integration tests for verifying API endpoints and database-related behavior.

Interactive API documentation is available through Swagger when running the app locally.

## Tech Stack
- ASP.NET Core (.NET 10)
- Entity Framework Core
- PostgreSQL
- Docker
- Docker Compose
## Quick start

#### Linux
1. Clone Repository
```sh
git clone git@github.com:Cavor1/Dotnet-Recipes-API.git
cd Dotnet-Recipes-API 
```
2. Create .env file
```sh
cp .env.example .env
```
3. Run with docker compose
```sh
docker compose up --build
```


## Tests
```sh
dotnet test
```

## Endpoints
#### Documentation
/swagger
#### IngredientsEndpoints
GET /ingredients  
POST /ingredients  
GET /ingredients/{id}  
PUT /ingredients/{id}  
DELETE /ingredients/{id}   
#### MealEndpoints
GET /meals  
POST /meals  
GET /meals/{id}  
PUT /meals/{id}  
DELETE /meals/{id}  
PATCH /meals/{id}/eat  
PATCH /meals/{id}/undoeat  
POST /meals/multiple  
#### RecipesEndpoints
GET /recipes  
POST /recipes  
GET /recipes/{id}  
PUT /recipes/{id}  
DELETE /recipes/{id}  
#### StatsEndpoints
GET /stats  
GET /stats/today  
