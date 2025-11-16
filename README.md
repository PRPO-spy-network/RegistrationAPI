# RegistrationAPI

RegistrationAPI je mikrostoritev, ki izpostavi endpoint /registration. Ta skrbi za registracijo novega avtomobila in pregled registracij. Komunicira predvsem z aplikacijo. 

## Postavitev

Aplikacjo najlažje postavimo z Dockerfile. Ko jo postavljamo moramo definirat nekaj okoljskih spremenljivk: 

- ASPNETCORE_HTTPS_PORTS : port na katerim mikrostoritev izpostavi https npr. 443
- ASPNETCORE_HTTP_PORTS : port na katerim mikrostoritev izpostavi http npr. 80
- TIMESCALE_CONN_STRING : povezava do podatkovne baze (\* podatkovna baza ni nujno timescale, ampak aplikacija je bila zasnovana na naèin, ki je zelo lagoden za njo)

Nastavimo lahko tudi vse spremenljivke v appsettings.json kot okoljske na enak naèin kot druge ASP.NET aplikacije. 