# AOW AIO Verwerking

## Appendix: `Row`
Ergens tijdens de latere stappen van de verwerking wordt er gebruik gemaakt van 
`Row` objecten. Het gebruik hiervan is enigszins onconventioneel binnen de
huidige codebase, vandaar wat nadere uitleg.

### Introductie
In essentie is een `Row` niets anders dan een `IDictionary<string, object?>`
en elk object van het type `Row` kan dus gebruikt worden als een *dictionary* met
dezelfde type argumenten (`<string, object?>`). Het is eenvoudig om een `Row` te 
maken op basic van zo'n dictionary (via de constructor) en omgekeerd (via de 
interface) en afgezien van wat nuance kunnen we een `Row` object beschouwen als 
een reguliere generic dictionary.

### Features
Objecten van het type `Row` hebben een paar extra features om hun gedrag
aan te passen en wat extra zaken die het net iets makkelijker maken om deze
objecten te gebruiken ten opzichte van een reguliere dictionary.

#### MissingKeyBehavior
Elk `Row` object heeft een `MissingKeyBehavior` waarmee wordt bepaald wat er 
gebeurt wanneer een sleutel niet gevonden wordt. Standaard staat dit op 
`MissingKeyBehavior.Ignore` waarbij er `null` wordt geretourneerd in het geval
dat een bepaalde sleutel niet gevonden wordt.

Een andere ingebouwde optie is `MissingKeyBehavior.Throw` waarbij er een 
`KeyNotFoundException` wordt afgevuurd. Dit komt overeen met het standaard 
gedrag van een dictionary en de exception is equivalent.

Het is tevens mogelijk om specifieke implementaties te gebruiken met behulp van
de `IMissingKeyBehavior` interface.

#### ValueConverter
Elk `Row` object heeft een `ValueConverter` eigenschap waarmee wordt 
aangegeven hoe waarden worden omgezet voordat ze aan de dictionary worden 
toegevoegd. 

> Deze actie vindt alleen maar plaats op het moment dat een waarde 
wordt toegevoegd (i.e. tijdens de `set` van de indexer).

Deze eigenschap wordt vaak gebruikt om *null* waardes te converteren
of te *wrappen* in een ander object. Een veel voorkomend scenario is 
bijvoorbeeld als je alle `null` waardes wilt converteren naar `DBNull.Value`,
een optional of een andere vervanging voor `null` of andere specifieke waarden
wilt omzetten naar een vervangend type (op globaal/record niveau, dit is geen 
handige ingang voor veld-specifieke conversies).

Gelijk aan `MissingKeyBehavior` is het mogelijk om een specifieke 
implementatie te gebruiken door middel van de `IValueConverter` interface. Dit
kan bijvoorbeeld handig zijn als je een nul-datum (`00000000`) wilt converteren
naar een `null` waarde of iets in die geest.

#### FromObject & ToObject
Het is vrij eenvoudig om een `Row` object te maken van een regulier object met
behulp van de `FromObject` method en omgekeerd om een `Row` om te vormen naar
een regulier object met behulp van de `ToObject` method.

> `FromObject` en `ToObject` laten altijd respectievelijk het object of het
> `Row` object ongewijzigd.

Een veel voorkomend scenario is bijvoorbeeld dat je begint met een object
van een bepaald type en dit moet omgevormd worden naar een object van een 
ander type. Vaak gebeurt het dat er voor deze transformatie context informatie
aan het originele object toegevoegd moet worden voordat het uiteindelijk naar
een ander type kan worden omgevormd.

```csharp
// From object (dict and dyn)
var obj = new
{
    Name = "Foo",
    Description = "Bar",
};

var dict = Row.FromObject(obj);
Console.WriteLine(dict["Name"]);
Console.WriteLine(dict["Description"]);

dynamic dyn = Row.FromObject(obj);
Console.WriteLine(dyn.Name);
Console.WriteLine(dyn.Description);

record Item(string Name, string Description);

var recDict = dict.ToObject<Item>();
Console.WriteLine(recDict.Name);
Console.WriteLine(recDict.Description);

var recDyn = dyn.ToObject<Item>();
Console.WriteLine(recDyn.Name);
Console.WriteLine(recDyn.Description);
```
#### Dynamic
Objecten van het type `Row` zijn dynamisch. Dit betekent dat ze niet alleen 
een dictionary zijn maar ook een object waarvan de eigenschappen dynamisch 
worden toegevoegd en verwijderd.

> Dit dynamische gedrag wordt alleen gebruikt indien een `Row` object als 
> `dynamic` gedeclareerd wordt.

```csharp
// Dictionary
var dict = new Row()
{
    ["Foo"] = "bar",
}
Console.WriteLine(dict["Foo"]);

// Dynamic
dynamic dyn = new Row()
{
    Foo = "bar",
}
Console.WriteLine(dyn.Foo);
```
In sommige scenario's (zeker tijdens prototyping) kan het handig zijn om op 
`dynamic` te leunen voor snelle ontwikkeling. Echter, `dynamic` is aanzienlijk
trager en in productiecode wil je gebruik hiervan beperken.

#### StringComparer
Niet per se een feature van `Row` maar een *heads-up* voor gebruikers van deze
klasse. Het is mogelijk om een alternatieve `StringComparer` op te geven via
de constructor. Dit kan handig zijn in scenario's waarin je bijvoorbeeld 
niet *case-sensitive* wilt vergelijken op keys. Deze comparer werkt zowel
in normale dictionary modus alswel in dynamische modus.

> Deze comparer wordt alleen gebruikt voor key lookup (tijdens
> de `get` van de indexer).

De standaard comparer is `StringComparer.InvariantCulture`.

## Tips
* Een `Row` object is van nature behoorlijk dynamisch. Als er bepaalde velden
zijn die een belangrijke rol spelen in het proces dan is het een goed idee om
de namen hiervan te definieren (als `static` of `const` in bijvoorbeeld een 
`WellKnownFields` type).
* Vergeet niet dat het mogelijk is om elk object of object structuur op te 
slaan in velden van een `Row` object. Het grootste probleem hier is om ze later 
in het proces weer naar de juiste types te converteren.
* Zelfs anonieme objecten zijn geen probleem - maar hoe krijg je ze eruit? Hier
zou `dynamic` een mogelijke oplossing zijn.
* Het is niet ongebruikelijk om `Row` objecten in velden van andere `Row` 
objecten op te slaan. Dit komt vaak voor in *aggregatie* of *join* scenario's. 
* Zolang inputs en outputs van een proces *typed* zijn is het meestal niet 
zo'n probleem om tijdens de interne verwerking vooral met `Row` objecten te 
werken. Als je verschillende stappen in je verwerkingsproces hebt dan kan het
soms best lastig zijn om allerlei namen te bedenken voor allerlei variaties van
je data. Zolang je gebruikt maakt van `Row` objecten hoef je over het algemeen 
niet al te creatief te zijn.
* Objecten van het type `Row` zijn behoorlijk makkelijk te gebruiken in unit
tests (met name door de `FromObject` en `ToObject` methods). Dit maakt het 
aantrekkelijk om een proces op te delen in kleine stappen (die gebruik maken van 
`Row` objecten) en al deze stappen individueel (of in combinatie) te testen.
* Als je een nieuwe `Row` maakt op basis van een bestaand `Row` object dan is
het een goed idee om de `Clone` method te gebruiken en zodoende niet de
bestaande `Row` aan te passen. Dit kan problemen helpen te voorkomen in
scenario's waarin meerdere threads aan hetzelfde proces werken.
* Maak gebruik van  `dynamic` tijdens ontwikkeling en prototyping, maar vermijd 
in productiecode waar mogelijk. Gebruik *normale* dictionary access en 
definieer constanten voor veldnamen.