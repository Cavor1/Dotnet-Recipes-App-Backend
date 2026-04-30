## Recipes API

ASP.NET Core API for managing recipes, meals, and calorie tracking.  

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
