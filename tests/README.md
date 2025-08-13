# Тесты TaskManagementSystem

## Структура тестов

```
tests/
├── AuthService.Tests/
│   ├── AuthServiceTests.cs              # Тесты сервиса аутентификации
│   ├── JwtTests.cs                      # Тесты JWT токенов
│   ├── PasswordTests.cs                 # Тесты хеширования паролей
│   └── UserTests.cs                     # Тесты репозитория пользователей
├── TaskService.Tests/
│   └── TaskTests.cs                     # Тесты работы с базой данных задач
├── NotificationService.Tests/
│   └── NotificationTests.cs             # Тесты контроллера уведомлений
└── Integration.Tests/
    └── ApiGatewayTests.cs               # Интеграционные тесты API Gateway
```



### 1. Названия методов тестов
Используем паттерн: `[Метод]_Should_[Ожидаемый_результат]_When_[Условие]`

Примеры:
- `Register_Should_Create_New_User_And_Return_Token()`
- `Login_Should_Return_Token_When_Correct_Password()`
- `GetUserByEmail_Should_Return_Null_When_User_Not_Found()`

### 2. Структура тестов (AAA Pattern)
```csharp
[Fact]
public async Task MethodName_Should_DoSomething()
{
    // Arrange - подготовка данных
    var testData = new TestObject { Property = "value" };
    
    // Act - выполнение действия
    var result = await service.Method(testData);
    
    // Assert - проверка результата
    result.Should().NotBeNull();
    result.Property.Should().Be("expected_value");
}
```

### 3. Комментарии на русском языке
- Используем простые комментарии для объяснения логики
- Группируем код по секциям Arrange/Act/Assert

### 4. Тестовые данные
- Используем понятные названия: `test@example.com`, `"Тестовая задача"`
- Избегаем абстрактных названий типа `"a@b.com"`, `"Test"`

### 5. Проверки (Assertions)
Используем FluentAssertions для читаемых проверок:
```csharp
// Хорошо
result.Should().NotBeNull();
result.Title.Should().Be("Ожидаемый заголовок");
items.Should().HaveCount(2);

// Плохо
Assert.NotNull(result);
Assert.Equal("Expected", result.Title);
Assert.Equal(2, items.Count);
```

## Типы тестов

### Unit Tests
- **AuthServiceTests**: Тестирование бизнес-логики аутентификации
- **JwtTests**: Тестирование генерации и валидации JWT токенов
- **PasswordTests**: Тестирование хеширования паролей
- **UserTests**: Тестирование работы с базой данных пользователей
- **TaskTests**: Тестирование CRUD операций с задачами
- **NotificationTests**: Тестирование API контроллера уведомлений

### Integration Tests
- **ApiGatewayTests**: Тестирование API Gateway и маршрутизации

## Запуск тестов

```bash
# Все тесты
dotnet test

# Конкретный проект
dotnet test tests/AuthService.Tests/

# С покрытием кода
dotnet test --collect:"XPlat Code Coverage"
```

## Особенности реализации

### InMemory Database
Используем InMemory базу данных для изоляции тестов:
```csharp
var options = new DbContextOptionsBuilder<JobsDbContext>()
    .UseInMemoryDatabase("unique-test-name")
    .Options;
```

### Mocking
Используем Moq для создания моков зависимостей:
```csharp
var mockService = new Mock<IService>();
mockService.Setup(x => x.Method()).Returns(expectedValue);
```
