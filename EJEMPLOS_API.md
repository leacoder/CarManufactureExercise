# Ejemplos de Uso de la API - CarManufactureAPI

## Todos los Endpoints Implementados

1. **POST /api/sales** - Crear una venta
2. **GET /api/sales/total-volume** - Obtener volumen total
3. **GET /api/sales/volume-by-center** - Obtener volumen por centro
4. **GET /api/sales/percentage-by-model-and-center** - Obtener porcentajes por modelo y centro

---

## Endpoint 1: POST /api/sales (Crear Venta)

El endpoint para crear ventas acepta el campo `carModel` de dos formas diferentes para mayor flexibilidad y facilidad de uso:

### Opcion 1: Usando el nombre del modelo (STRING)

Esta opcion es mas intuitiva y clara para el usuario:

```json
POST /api/sales
Content-Type: application/json

{
  "carModel": "Sedan",
  "distributionCenterId": 1,
  "quantity": 5
}
```

#### Modelos disponibles (case-insensitive):
- `"Sedan"` - Automovil sedan
- `"SUV"` - Vehiculo SUV
- `"Offroad"` - Vehiculo todoterreno
- `"Sport"` - Automovil deportivo

**Nota:** El nombre del modelo NO es sensible a mayusculas/minusculas. Puedes usar:
- `"sedan"`, `"SEDAN"`, `"Sedan"`, `"SeDaN"` - todos son validos

### Opcion 2: Usando el indice numerico (INT como STRING)

Si prefieres usar indices numericos, tambien puedes hacerlo:

```json
POST /api/sales
Content-Type: application/json

{
  "carModel": "0",
  "distributionCenterId": 1,
  "quantity": 5
}
```

#### Indices disponibles:
- `"0"` = Sedan
- `"1"` = SUV
- `"2"` = Offroad
- `"3"` = Sport

## Ejemplos Completos

### Ejemplo 1: Crear venta de SUV usando nombre
```bash
curl -X POST http://localhost:5000/api/sales \
  -H "Content-Type: application/json" \
  -d '{
    "carModel": "SUV",
    "distributionCenterId": 2,
    "quantity": 3
  }'
```

**Respuesta exitosa (201 Created):**
```json
{
  "id": 1,
  "carModel": "SUV",
  "distributionCenterId": 2,
  "distributionCenterName": "Centro Sur",
  "quantity": 3,
  "unitPrice": 12000.00,
  "totalAmount": 36000.00,
  "saleDate": "2025-10-25T10:30:00Z",
  "message": "Venta creada exitosamente"
}
```

### Ejemplo 2: Crear venta de Sport usando indice
```bash
curl -X POST http://localhost:5000/api/sales \
  -H "Content-Type: application/json" \
  -d '{
    "carModel": "3",
    "distributionCenterId": 1,
    "quantity": 2
  }'
```

**Respuesta exitosa (201 Created):**
```json
{
  "id": 2,
  "carModel": "Sport",
  "distributionCenterId": 1,
  "distributionCenterName": "Centro Norte",
  "quantity": 2,
  "unitPrice": 25000.00,
  "totalAmount": 50000.00,
  "saleDate": "2025-10-25T10:35:00Z",
  "message": "Venta creada exitosamente"
}
```

### Ejemplo 3: Error - Modelo invalido
```bash
curl -X POST http://localhost:5000/api/sales \
  -H "Content-Type: application/json" \
  -d '{
    "carModel": "Toyota",
    "distributionCenterId": 1,
    "quantity": 1
  }'
```

**Respuesta de error (400 Bad Request):**
```json
{
  "error": "El modelo 'Toyota' no es valido. Los modelos validos son: Sedan, SUV, Offroad, Sport"
}
```

### Ejemplo 4: Error - Indice invalido
```bash
curl -X POST http://localhost:5000/api/sales \
  -H "Content-Type: application/json" \
  -d '{
    "carModel": "99",
    "distributionCenterId": 1,
    "quantity": 1
  }'
```

**Respuesta de error (400 Bad Request):**
```json
{
  "error": "El indice 99 no corresponde a ningun modelo valido. Los valores validos son: 0 (Sedan), 1 (SUV), 2 (Offroad), 3 (Sport)"
}
```
---

## Endpoint 2: GET /api/sales/total-volume (Volumen Total)

Obtiene el volumen total de ventas sumando todas las unidades y montos.

### Ejemplo de uso

```bash
curl -X GET http://localhost:5000/api/sales/total-volume
```

### Respuesta exitosa (200 OK)

```json
{
  "totalUnits": 65,
  "totalAmount": 785000.00,
  "totalSales": 20
}
```

### Descripcion de campos

- **totalUnits**: Total de unidades vendidas (suma de todas las cantidades)
- **totalAmount**: Monto total en dolares USD de todas las ventas
- **totalSales**: Cantidad total de transacciones de venta

---

## Endpoint 3: GET /api/sales/volume-by-center (Volumen por Centro)

Obtiene el volumen de ventas agrupado por cada centro de distribucion.

### Ejemplo de uso

```bash
curl -X GET http://localhost:5000/api/sales/volume-by-center
```

### Respuesta exitosa (200 OK)

```json
{
  "centers": [
    {
      "distributionCenterId": 1,
      "distributionCenterName": "Centro Norte",
      "totalUnits": 18,
      "totalAmount": 215000.00,
      "totalSales": 6
    },
    {
      "distributionCenterId": 2,
      "distributionCenterName": "Centro Sur",
      "totalUnits": 22,
      "totalAmount": 280000.00,
      "totalSales": 7
    },
    {
      "distributionCenterId": 3,
      "distributionCenterName": "Centro Este",
      "totalUnits": 15,
      "totalAmount": 190000.00,
      "totalSales": 4
    },
    {
      "distributionCenterId": 4,
      "distributionCenterName": "Centro Oeste",
      "totalUnits": 10,
      "totalAmount": 100000.00,
      "totalSales": 3
    }
  ],
  "grandTotalUnits": 65,
  "grandTotalAmount": 785000.00
}
```

### Descripcion

Este endpoint permite analizar el desempeno de cada centro de distribucion:
- Unidades vendidas por centro
- Monto total recaudado por centro
- Cantidad de ventas realizadas en cada centro
- Totales globales para comparacion

---

## Endpoint 4: GET /api/sales/percentage-by-model-and-center (Porcentajes)

Obtiene el porcentaje de unidades de cada modelo vendido en cada centro sobre el total de ventas.

### Ejemplo de uso

```bash
curl -X GET http://localhost:5000/api/sales/percentage-by-model-and-center
```

### Respuesta exitosa (200 OK)

```json
{
  "centers": [
    {
      "distributionCenterId": 1,
      "distributionCenterName": "Centro Norte",
      "totalUnitsInCenter": 18,
      "models": [
        {
          "carModel": "Offroad",
          "unitsSold": 3,
          "percentageOfCenter": 16.67,
          "percentageOfTotal": 4.62
        },
        {
          "carModel": "SUV",
          "unitsSold": 5,
          "percentageOfCenter": 27.78,
          "percentageOfTotal": 7.69
        },
        {
          "carModel": "Sedan",
          "unitsSold": 8,
          "percentageOfCenter": 44.44,
          "percentageOfTotal": 12.31
        },
        {
          "carModel": "Sport",
          "unitsSold": 2,
          "percentageOfCenter": 11.11,
          "percentageOfTotal": 3.08
        }
      ]
    },
    {
      "distributionCenterId": 2,
      "distributionCenterName": "Centro Sur",
      "totalUnitsInCenter": 22,
      "models": [
        {
          "carModel": "Offroad",
          "unitsSold": 7,
          "percentageOfCenter": 31.82,
          "percentageOfTotal": 10.77
        },
        {
          "carModel": "SUV",
          "unitsSold": 9,
          "percentageOfCenter": 40.91,
          "percentageOfTotal": 13.85
        },
        {
          "carModel": "Sedan",
          "unitsSold": 4,
          "percentageOfCenter": 18.18,
          "percentageOfTotal": 6.15
        },
        {
          "carModel": "Sport",
          "unitsSold": 2,
          "percentageOfCenter": 9.09,
          "percentageOfTotal": 3.08
        }
      ]
    }
  ],
  "totalUnitsGlobal": 65
}
```

### Descripcion de campos

- **percentageOfCenter**: Porcentaje del modelo sobre el total del centro
- **percentageOfTotal**: Porcentaje del modelo sobre el total global de todas las ventas
- **unitsSold**: Cantidad de unidades vendidas de ese modelo en ese centro