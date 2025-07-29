# IdMatrix SOAP Library

A C# library for integrating with Equifax IdMatrix SOAP API.

## Features

- SOAP envelope generation
- JSON to SOAP conversion
- Response parsing
- Comprehensive error handling
- Unit and integration tests

## Usage

```csharp
var client = new SoapClient("username", "password");

var request = new IdMatrixRequest
{
    ClientReference = "TEST_CLIENT_1",
    Reason = "Identity verification",
    FamilyName = "Smith",
    FirstGivenName = "John",
    DateOfBirth = new DateTime(1980, 1, 1),
    // ... other properties
};

var response = await client.SendRequestAsync(request);
```

## Testing

Run tests with:
```bash
dotnet test
```

## Configuration

Set credentials via constructor or configuration file (not included in git).