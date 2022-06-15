# Turnit Store

## Overview
This is a simple product inventory application. Main concepts are **Product**, **Category** and **Store**.
Product may belong to multiple categories or it can be uncategorized. 

Each store (warehouse) have it's own availability of products.

## API methods

### GET /categories
Should return list of all the available categories in alphabetical order.

Response:
```
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "string"
  }
]
```

### GET /products
Should return list of all the available products per category (also uncategorized).

Response:
```
[
  {
    "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "products": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "string",
        "availability": [
          {
            "storeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "availability": 0
          }
        ]
      }
    ]
  }
]
```

### GET /products/by-category/{categoryId}
Should return list af all the available products in the specific category.

Response:
```
[
  {
    "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "products": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "string",
        "availability": [
          {
            "storeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "availability": 0
          }
        ]
      }
    ]
  }
]
```

### PUT /products/{productId}/category/{categoryId}
Adds specific product to the category. Should be idempotent.

### DELETE /products/{productId}/category/{categoryId}
Removes specific product from the category. Should be idempotent.

### POST /products/{productId}/book
Marks products as booked in the store. This basically means that this quantity is sold (or reserved) to some client
and it should not be available as general availability.  

One request can contain quantities for multiple stores.

Request:
```
[
  {
    "storeId": "4f10a98a-e65b-11ec-a1ac-24ee9a88c06f",
    "quantity": 100
  }
]
```

### GET /stores
Should return list of all the available stores.

Response:
```
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "string"
  }
]
```

### POST /store/{storeId}/restock
Adds specified quantity of products to the stores availability.
This is used to increase the availability of products in specific store.

One request can contain quantities for multiple products (in the same store).

Request:
```
[
  {
    "productId": "4f10a98a-e65b-11ec-a1ac-24ee9a88c06f",
    "quantity": 100
  }
]
```

## Technical
Database layer is implemented using NHibernate and PostgreSQL database.

Application can be run with docker compose `docker-compose up` (docker-compose file is in the root directory). Application is exposed on the port 5000 and PostgreSQL is exposed on port 5632.

When running application locally (without docker, in dev environment), 
docker compose can be used to serve database and then can use connection string:
`Server=localhost;Port=5632;Database=turnit_store;User ID=postgres;Password=postgres`.

Application image can be built with (when using with docker-compose) `docker-compose build web`.

Swagger is accessible from the url http://localhost:5000/swagger.