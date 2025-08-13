# Task Management System

## Описание проекта

Данный проект представляет собой микросервисную систему управления задачами, разработанную в качестве тестового задания для демонстрации навыков работы с современными технологиями разработки программного обеспечения.

## Цель проекта

Основной целью данного проекта является демонстрация понимания и практического применения следующих концепций и технологий:
- Микросервисная архитектура
- Clean Architecture принципы
- REST API разработка
- Аутентификация и авторизация
- База данных и ORM
- Тестирование
- Документирование API

## Архитектурное решение

### Обоснование выбора микросервисной архитектуры

Микросервисная архитектура была выбрана по следующим причинам:
- **Масштабируемость**: Каждый сервис может масштабироваться независимо
- **Технологическая гибкость**: Возможность использования различных технологий для разных сервисов
- **Независимое развертывание**: Сервисы могут обновляться независимо друг от друга
- **Отказоустойчивость**: Сбой одного сервиса не приводит к полной остановке системы

### Структура решения

```
TaskManagementSystem/
├── src/
│   ├── ApiGateway.API/           # API Gateway на базе Ocelot
│   ├── AuthService/              # Сервис аутентификации и авторизации
│   │   ├── AuthService.API/      # Web API слой
│   │   ├── AuthService.Application/ # Бизнес-логика
│   │   ├── AuthService.Domain/   # Доменная модель
│   │   └── AuthService.Infrastructure/ # Инфраструктурный слой
│   ├── TaskService/              # Сервис управления задачами
│   │   ├── TaskService.API/
│   │   ├── TaskService.Application/
│   │   ├── TaskService.Domain/
│   │   └── TaskService.Infrastructure/
│   ├── NotificationService/      # Сервис уведомлений
│   │   ├── NotificationService.API/
│   │   ├── NotificationService.Application/
│   │   ├── NotificationService.Domain/
│   │   └── NotificationService.Infrastructure/
│   └── Common.Common/            # Общие компоненты и утилиты
├── tests/                        # Модульные и интеграционные тесты
├── docs/                         # Документация и Postman коллекции
└── scripts/                      # Скрипты автоматизации
```

## Технологический стек

### Основные технологии
- **.NET 8** - Современная платформа разработки с длительной поддержкой
- **ASP.NET Core** - Высокопроизводительный веб-фреймворк
- **Entity Framework Core** - ORM для работы с реляционными базами данных

### Базы данных
- **PostgreSQL** - Надежная реляционная СУБД для продакшена
- **InMemory Database** - Временная БД для разработки и тестирования

### Дополнительные компоненты
- **Ocelot** - API Gateway для маршрутизации запросов
- **SignalR** - Реализация real-time уведомлений
- **JWT** - Стандарт для аутентификации на основе токенов
- **Serilog** - Структурированное логирование
- **OpenTelemetry** - Инструментирование и мониторинг

### Тестирование
- **xUnit** - Фреймворк для модульного тестирования
- **ASP.NET Core Test Host** - Интеграционное тестирование

## Принципы Clean Architecture

Каждый сервис реализован в соответствии с принципами Clean Architecture:

### Domain Layer
- Содержит бизнес-модели и интерфейсы
- Не зависит от внешних слоев
- Определяет контракты для взаимодействия

### Application Layer
- Реализует бизнес-логику
- Координирует взаимодействие между доменными объектами
- Использует интерфейсы из Domain Layer

### Infrastructure Layer
- Реализует интерфейсы из Domain Layer
- Обеспечивает взаимодействие с внешними системами
- Содержит конфигурацию Entity Framework

### API Layer
- Предоставляет HTTP endpoints
- Обрабатывает HTTP запросы и ответы
- Валидирует входящие данные

## Инструкции по запуску

### Предварительные требования
- .NET 8 SDK
- PostgreSQL (для продакшена)
- PowerShell (для Windows)

### Вариант 1: Разработка с InMemory базами данных

```bash
# Клонирование репозитория
git clone <repository-url>
cd TaskManagementSystem

# Запуск всех сервисов
dotnet run --project src/ApiGateway.API
dotnet run --project src/AuthService/AuthService.API
dotnet run --project src/TaskService/TaskService.API
dotnet run --project src/NotificationService/NotificationService.API
```

### Вариант 2: Продакшен с PostgreSQL

```bash
# 1. Установка PostgreSQL
# 2. Создание баз данных
.\scripts\create_postgres_databases.ps1

# 3. Настройка переменных окружения
$env:JWT_SECRET_KEY="your-secret-key-here"
$env:JWT_ISSUER="TaskManagementSystem"
$env:JWT_AUDIENCE="TaskManagementSystem"

# 4. Запуск сервисов
dotnet run --project src/ApiGateway.API
dotnet run --project src/AuthService/AuthService.API
dotnet run --project src/TaskService/TaskService.API
dotnet run --project src/NotificationService/NotificationService.API
```

### Автоматизация создания баз данных

Скрипт `scripts/create_postgres_databases.ps1` автоматизирует процесс создания необходимых баз данных:

**Параметры:**
- `-DbHost` - хост PostgreSQL (по умолчанию: "localhost")
- `-Port` - порт PostgreSQL (по умолчанию: 5432)
- `-Username` - имя пользователя (по умолчанию: "postgres")
- `-Password` - пароль (по умолчанию: "your_password")
- `-Owner` - владелец баз данных (по умолчанию: "postgres")
- `-Databases` - массив имен баз данных

**Создаваемые базы данных:**
- `TaskManagement_Auth` - для сервиса аутентификации
- `TaskManagement_Tasks` - для сервиса управления задачами
- `TaskManagement_Notifications` - для сервиса уведомлений

## API Endpoints

### API Gateway
- **URL**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

### Auth Service
- **URL**: http://localhost:5001
- **Swagger**: http://localhost:5001/swagger

| Метод | Endpoint | Описание | Требования |
|-------|----------|----------|------------|
| POST | `/api/auth/register` | Регистрация нового пользователя | Валидация email, пароля |
| POST | `/api/auth/login` | Аутентификация пользователя | Проверка учетных данных |

### Task Service
- **URL**: http://localhost:5002
- **Swagger**: http://localhost:5002/swagger

| Метод | Endpoint | Описание | Авторизация |
|-------|----------|----------|-------------|
| GET | `/api/tasks` | Получение списка задач | JWT токен |
| GET | `/api/tasks/{id}` | Получение задачи по идентификатору | JWT токен |
| POST | `/api/tasks` | Создание новой задачи | JWT токен |
| PUT | `/api/tasks/{id}` | Обновление существующей задачи | JWT токен |
| DELETE | `/api/tasks/{id}` | Удаление задачи | JWT токен |

### Notification Service
- **URL**: http://localhost:5003
- **Swagger**: http://localhost:5003/swagger
- **SignalR Hub**: http://localhost:5003/hubs/notifications

| Метод | Endpoint | Описание | Авторизация |
|-------|----------|----------|-------------|
| GET | `/api/notifications` | Получение уведомлений | JWT токен |
| POST | `/api/notifications` | Создание уведомления | JWT токен |
| PUT | `/api/notifications/{id}` | Обновление уведомления | JWT токен |

## Система аутентификации

### Реализация JWT аутентификации

Система использует JWT (JSON Web Tokens) для обеспечения безопасной аутентификации пользователей.

#### Процесс получения токена

```bash
# Регистрация пользователя
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "password123"
  }'

# Аутентификация пользователя
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "password123"
  }'
```

#### Использование токена для авторизованных запросов

```bash
curl -X GET http://localhost:5000/api/tasks \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Мониторинг и диагностика

### Health Checks
Реализованы health checks для всех сервисов:
- API Gateway: http://localhost:5000/health
- Auth Service: http://localhost:5001/health
- Task Service: http://localhost:5002/health
- Notification Service: http://localhost:5003/health

### Метрики Prometheus
Поддержка метрик Prometheus для мониторинга производительности:
- API Gateway: http://localhost:5000/metrics
- Auth Service: http://localhost:5001/metrics
- Task Service: http://localhost:5002/metrics
- Notification Service: http://localhost:5003/metrics

## Тестирование

### Запуск тестов

```bash
# Запуск всех тестов
dotnet test

# Запуск тестов конкретного сервиса
dotnet test tests/AuthService.Tests/
dotnet test tests/TaskService.Tests/
dotnet test tests/NotificationService.Tests/
dotnet test tests/Integration.Tests/
```

### Покрытие тестами
- Модульные тесты для бизнес-логики
- Интеграционные тесты для API endpoints
- Тесты для репозиториев и сервисов

## Документация API

### Postman Collection
Предоставлена готовая Postman коллекция в файле `docs/TaskManagementSystem.postman_collection.json`

### Переменные окружения Postman
Файл `docs/TaskManagementSystem.postman_environment.json` содержит настройки переменных для Postman

## Конфигурация

### Переменные окружения

```bash
# JWT настройки
JWT_SECRET_KEY=your-secret-key
JWT_ISSUER=TaskManagementSystem
JWT_AUDIENCE=TaskManagementSystem

# Строки подключения к базам данных
DefaultConnection=Host=localhost;Port=5432;Database=TaskManagement_Tasks;Username=postgres;Password=your_password
PostgresAdmin=Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=your_password
```

### Файл конфигурации разработки

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TaskManagement_Tasks;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "TaskManagementSystem",
    "Audience": "TaskManagementSystem"
  }
}
```

## Общие компоненты (Common.Common)

### Модули
- **Error** - Централизованная обработка ошибок
- **Tracing** - Корреляция запросов между сервисами
- **Health** - Стандартизированные health checks
- **Db** - Утилиты для работы с базами данных
- **Responses** - Стандартизированные ответы API
- **Validation** - Валидация входящих запросов

