### Recipes API

ASP.NET Core API for managing recipes, meals, and calorie tracking.  
Uses PostgreSQL as database.

### Quick start
```sh
git clone git@github.com:Cavor1/Dotnet-Recipes-API.git
cd Dotnet-Recipes-API 
cp .env.example .env
docker compose up --build
```

### Endpoints
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
