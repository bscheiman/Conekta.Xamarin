# Conekta.Xamarin
Tokenizador para Conekta / Xamarin

## Instalación

Install-Package Conekta.Xamarin

## Uso

```cs
string token = await new ConektaTokenizer("llave publica").GetTokenAsync("cardNumber", "name", "cvc", year, month);
string token = await new ConektaTokenizer("llave publica").GetTokenAsync("cardNumber", "name", "cvc", new DateTime(year, month, 1));
```
