# Car Manufacture API

API REST para gestión de ventas de una fábrica de automóviles con 4 modelos de vehículos y 4 centros de distribución.

## Descripción

Esta API permite registrar ventas de automóviles en diferentes centros de distribución.

### Modelos de Automóviles y Precios

| Modelo | Precio Base | Impuesto | Precio Final |
|--------|-------------|----------|--------------|
| Sedan  | $8,000 USD  | -        | $8,000 USD   |
| SUV    | $9,500 USD  | -        | $9,500 USD   |
| Offroad| $12,500 USD | -        | $12,500 USD  |
| Sport  | $18,200 USD | 7%       | $19,474 USD  |

**Nota:** El modelo Sport incluye un impuesto adicional del 7% sobre su precio base.

### Centros de Distribución

La empresa cuenta con 4 centros de distribución:
1. Centro Norte (Buenos Aires Norte)
2. Centro Sur (Buenos Aires Sur)
3. Centro Este (Región Este)
4. Centro Oeste (Región Oeste)

## Arquitectura y Patrones

### Estructura del Proyecto

```
CarManufactureAPI/
├── Controllers/        # Controladores REST
│   └── SalesController.cs
├── Services/          # Lógica de negocio
│   ├── ISalesService.cs
│   └── SalesService.cs
├── Repositories/      # Acceso a datos
│   ├── ISalesRepository.cs
│   └── InMemorySalesRepository.cs
├── Models/            # Entidades de dominio
│   ├── CarModelType.cs
│   ├── CarModelPricing.cs
│   ├── DistributionCenter.cs
│   └── Sale.cs
├── DTOs/              # Data Transfer Objects
│   ├── CreateSaleRequest.cs
│   ├── SaleResponse.cs
│   ├── TotalVolumeResponse.cs
│   ├── VolumeByCenterResponse.cs
│   └── PercentageByModelAndCenterResponse.cs
└── Filters/           # Filtros personalizados
    └── PerformanceFilter.cs

CarManufactureAPI.Tests/
├── Services/
└── Repositories/
```

### Patrones Implementados

1. **Repository Pattern**: Abstracción del acceso a datos mediante interfaces (`ISalesRepository`)
2. **Dependency Injection**: Inyección de dependencias nativa de .NET Core
3. **Service Layer**: Separación de lógica de negocio en servicios
4. **DTO Pattern**: Objetos específicos para transferencia de datos en la API
5. **Action Filter**: Filtro personalizado para medición de performance

### Decisiones Técnicas

#### ¿Por qué .NET 8?
- Framework moderno y performante
- Soporte nativo para inyección de dependencias
- Integración perfecta con Swagger/OpenAPI
- Excelente para APIs REST

#### ¿Por qué Repository Pattern?
- Facilita el testing mediante mocks
- Permite cambiar la implementación de persistencia sin afectar la lógica de negocio
- Actualmente usa almacenamiento en memoria, pero podría migrarse fácilmente a una base de datos

#### ¿Por qué un filtro de performance personalizado?
- Cumple el requerimiento de "Imprimir el tiempo de ejecución de cada método"
- Se aplica automáticamente a todos los endpoints del controlador
- Registra tiempos en logs y headers HTTP
- No contamina el código de los controladores

#### ¿Por qué almacenamiento en memoria sin datos pre-cargados?
- El repositorio inicia vacío para un estado limpio
- Los datos se agregan dinámicamente mediante el endpoint POST
- Los tests crean sus propios datos mockeados según sea necesario
- Facilita el testing unitario con datos controlados

## Endpoints de la API

### 1. Crear una Venta ✅

```http
POST /api/sales
Content-Type: application/json

{
  "carModel": "Sedan",  // Sedan | SUV | Offroad | Sport o del 1 al 4
  "distributionCenterId": 1,  // 1 a 4
  "quantity": 5
}
```

**Respuesta (201 Created):**
```json
{
  "id": 21,
  "carModel": "Sedan",
  "distributionCenterId": 1,
  "distributionCenterName": "Centro Norte",
  "quantity": 5,
  "unitPrice": 8000,
  "totalAmount": 40000,
  "saleDate": "2025-10-24T19:45:00Z",
  "message": "Venta creada exitosamente"
}
```

**Validaciones:**
- El modelo de automóvil debe ser válido (Sedan, SUV, Offroad, Sport) o del 1 al 4
- El centro de distribución debe existir (1-4)
- La cantidad debe ser mayor a 0

### 2. Obtener Volumen Total de Ventas ✅

```http
GET /api/sales/total-volume
```

**Respuesta (200 OK):**
```json
{
  "totalUnits": 65,
  "totalAmount": 785000.00,
  "totalSales": 20
}
```

**Descripción:**
- `totalUnits`: Total de unidades vendidas (suma de todas las cantidades)
- `totalAmount`: Monto total en dólares USD de todas las ventas
- `totalSales`: Cantidad total de transacciones de venta

### 3. Obtener Volumen de Ventas por Centro ✅

```http
GET /api/sales/volume-by-center
```

**Respuesta (200 OK):**
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

**Descripción:**
Retorna el volumen de ventas agrupado por cada centro de distribución, incluyendo:
- Total de unidades vendidas por centro
- Monto total de ventas por centro
- Cantidad de ventas por centro
- Totales globales (suma de todos los centros)

### 4. Obtener Porcentajes por Modelo y Centro ✅

```http
GET /api/sales/percentage-by-model-and-center
```

**Respuesta (200 OK):**
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

**Descripción:**
Retorna el porcentaje de unidades de cada modelo vendido en cada centro:
- `percentageOfCenter`: Porcentaje del modelo sobre el total del centro
- `percentageOfTotal`: Porcentaje del modelo sobre el total global de todas las ventas
- Permite analizar qué modelos son más populares en cada centro

## Medición de Performance

Todos los endpoints incluyen medición automática de tiempo de ejecución:

1. **En los logs de la aplicación:**
```
[PERFORMANCE] Iniciando ejecución: Sales.CreateSale
[PERFORMANCE] Tiempo de ejecución: Sales.CreateSale - 15ms
```

2. **En el header HTTP de respuesta:**
```
X-Execution-Time-Ms: 15
```

## Cómo Ejecutar la Aplicación

### Requisitos Previos
- .NET 8.0 SDK o superior
- (Opcional) Visual Studio 2022, VS Code o JetBrains Rider

### Pasos de Ejecución

1. **Clonar el repositorio:**
```bash
git clone <repository-url>
cd CarManufactureExercise
```

2. **Restaurar dependencias:**
```bash
cd CarManufactureAPI
dotnet restore
```

3. **Ejecutar la aplicación:**
```bash
dotnet run
```

4. **Acceder a Swagger UI:**
   - Abrir el navegador en: `https://localhost:7206` o `http://localhost:5175`
   - Swagger UI se mostrará automáticamente en la raíz

### Ejecutar con Visual Studio
1. Abrir `CarManufactureAPI.sln`
2. Presionar `F5` o hacer clic en "Start"
3. El navegador se abrirá automáticamente con Swagger UI

## Cómo Ejecutar los Tests

### Ejecutar Todos los Tests
```bash
cd CarManufactureAPI.Tests
dotnet test
```

### Tests Implementados

**Total: 23 tests unitarios** - Todos pasando ✅

#### Repositories Tests (10 tests)
- `InMemorySalesRepositoryTests`: Operaciones CRUD, queries, y agrupaciones
  - AddSale con cálculo de precios
  - GetDistributionCenter (válido e inválido)
  - GetAllSales
  - GetSalesByCenter
  - GetAllDistributionCenters
  - GetSalesGroupByCenter
  - GetSalesGroupByCenterAndModel

#### Services Tests (13 tests)
- `SalesServiceTests`: Lógica de negocio completa
  - CreateSale (con nombre y con índice numérico)
  - Validaciones de modelos y centros
  - GetTotalVolume (con y sin datos)
  - GetVolumeByCenter (múltiples centros, centros sin ventas)
  - GetPercentageByModelAndCenter (cálculo de porcentajes, casos sin datos)

### Cobertura de Código

| Componente | Cobertura | Notas |
|------------|-----------|-------|
| **SalesService** | 100% | Toda la lógica de negocio cubierta |
| **InMemorySalesRepository** | 100% | Todos los métodos testeados |
| **Models (Sale, DistributionCenter)** | 100% | Lógica de dominio cubierta |
| **DTOs** | 100% | Objetos de transferencia cubiertos |
| **Controllers** | No medido | Testeados indirectamente a través de servicios |

**Nota:** La cobertura se enfoca en la lógica de negocio y repositorios. Los controllers se testean indirectamente a través de los servicios.

## Datos en Memoria

La API utiliza almacenamiento en memoria y **inicia sin datos pre-cargados**. Las ventas se crean dinámicamente a través del endpoint POST `/api/sales`.

Para probar la API con datos:
1. Inicia la aplicación
2. Usa el endpoint POST para crear ventas
3. Consulta los endpoints GET para ver los resultados

Hay un metodo que mockea 20 ventas que se puede descomentar para iniciar con datos de ventas ya creados.
Los tests unitarios crean sus propios datos mockeados según sea necesario para cada escenario de prueba.

## Tecnologías Utilizadas

- **.NET 8.0**: Framework principal
- **ASP.NET Core**: Framework web
- **Swashbuckle**: Generación de Swagger/OpenAPI
- **xUnit**: Framework de testing
- **Moq**: Librería para mocking en tests
- **FluentAssertions**: Assertions expresivas para tests
- **Coverlet**: Herramienta de cobertura de código