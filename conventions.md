# definities
PascalCase: eerste letter van elk woord een hoofdletter.
camelCase: eerste letter van het eerste woord een kleine letter, eerste letter van de rest van de woorden een hoofdletter.
local variables: variables gedefinieerd in een function body.
underscore: '_'.
reverse domain notation: de root namespace is de domijn maar dan omgekeerd. bijvoorbeeld wordt google.com, com.google, gevolgd door het pad naar de file.
deadlock: wanneer verschillende operaties niet verder kunnen gaan omdat ze allemaal op elkaar wachten.

# naming rules
- Identifiers starten met een letter of underscore.
- Een identifier kan Unicode letter characters, decimal digit characters, Unicode connecting characters, Unicode combining characters, of Unicode formatting characters.
- Als je een identifier die een keyword matched definieerd moet er een @ voor, dit is voor samenwerking tussen verschillende talen.

# naming conventions
Generale regels over de namen van cs constructs.
- voor alles dat over 'public' identifiers gaat geld ook voor 'protected' en 'protected internal' identifiers.
- Attribute types eindigen met 'Attribute'.
voorbeeld:
```cs
public int ExampleAttributeAttribute { get; set; }
```
- Enum's hebben een singular noun voor nonflags(denk alles dat niet een optie is) en een plural noun voor flags.
voorbeeld:
```cs
public enum ExampleFlags
{
}

public enum ExampleNonFlag
{
}
```
- Een identifier moet niet meer dan 2 underscores('_') achter elkaar hebben.
voorbeeld:
```cs
// Good code
int x = 1;
private string _privateString = "";

// Bad code
int __x = 1;
private string __privateString = "";
```
- Gebruik meaningful and descriptive namen voor variables, classes en methods, dus niet "x" maar bijvoorbeeld "XCoordinate".
- Geef een voorkeur aan duidelijkheid over kortheid.
- Private en internal non-constant instance fields starten met een underscore.
voorbeeld:
```cs
private int _privateInt = 0;
internal sring _internalString = "";
```
- Static fields prefix je met 's_'.
voorbeeld:
```cs
static string s_StaticString = "";
```
- Gebruik zo min mogelijk afkortingen in namen, behalve voor algemeen afgesproken afkortingen(denk i voor in een for loop).
- Gebruik meaningful and descriptive namen voor namespaces, volg the reverse domain name notation, de root namespace is het domijn.
- Kies assembly names die het primaire doel van de assembly beschrijven.
- Vermijdt het gebruik van 1 leterige namen, behalve voor simpele loop counters.

## Pascal Case
Dit gaat over waar je PascalCase moet gebruiken voor een naam.
- gebruik PascalCase wanneer je een class, interface, struct of delegate een naam geeft.
voorbeelden:
``` cs
public class ExampleClass
{
}

public record ExampleRecord(
  int Value,
  char Letter);

public struct ExampleStruct
{
}

public delegate void ExampleDelegate(string argument);
```
- Wanneer je een interface een naam geeft gebruik je PascalCase en prefix je de naam met een 'I'.
voorbeeld:
```cs
public interface IExampleInterface
{
}
```
- Wanneer je een naam geeft aan public members van types, zoals fields, properties, events, gebruik je PascalCase. Gebruik ook PascalCase voor alle methods en functions.
voorbeeld:
```cs
public class ExampleEvents
{
  public bool IsValid;
  
  public byte Identifier { get; init; }
  
  public event Action EventProcessing;
  
  public void StartEventProcessing() 
  {
    static int ExampleLocalFunction() => Identifier;
  }
}
```
- Bij het maken van een record moet je PascalCase gebruiken voor de parameters omdat deze public zijn.
voorbeeld:
```cs
public record ExampleRecord(
  string Name,
  byte Id);
```
- Gebruik PascalCase voor alle constants, ongeacht de access modifier.
voorbeeld:
```cs
public const string ExampleString = "";
private const int ExampleInt = 1;
protected const char ExampleChar = 'a';
```

## camelCase
Dit gaat over waar je camelCase moet gebruiken voor een naam en wat extra regels voor naamgeving.
- Gebruik camelCase bij het geven van een naam aan private of internal non-constant fields en prefix ze met een underscore.
voorbeeld:
```cs
public class ExampleClass
{
  private string _fieldName;
  internal bool _isSet;
}
```
- Gebruik camelCase voor de namen van local variables, inclusief instances van een delegate type.
voorbeeld:
```cs
public delegate void ExampleDelegate(string argument);

public void ExampleMethod(byte identifier) 
{
  int exampleLocalvariable = 0;
  ExampleDelegate exampleDelegateInstance = (argument) => Console.WriteLine(argument);
}
```
- Static fields die private of internal zijn prefix je met 's_' en thread static fields prefix je met 't_'.
voorbeeld:
```cs
public class ExampleClass
{
  private static byte s_instanceIdentifier;
  [ThreadStatic]
  private static byte t_threadIdentifier;

  internal static string s_className;
  [ThreadStatic]
  internal static string t_theadName;
}
```
- Voor method parameters moet je camelCase gebruiken, dit geld ook voor parameters van constructors.
voorbeeld:
```cs
public void ExampleMethod(string[] methodArguments)
{
}
```
- Bij primary constructor parameters voor classes en structs gebruik je camelCase, omdat dit constant is met andere method parameters.
voorbeeld:
```cs
public class ExampleClass(string message)
{
  public ExampleClass()
  {
    Console.WriteLine(message);
  }
}
public struct ExampleStruct(int xCoordinate, int yCoordinate)
{
  public double Distance => Math.Sqrt(xCoordinate * xCoordinate + yCoordinate * yCoordinate);
}
```

## Type parameters
Regels over hoe je Type parameters een naam moet geven.
- Je mag een 1 leterige naam gebruiken bij type parameters als een duidelijkere naam geen extra waarde toevoegd, in dit geval moet je wel overwegen om 'T' te gebruiken.
voorbeeld:
```cs
public int IComparer<T>() => 0;
public delegate bool Predicate<T>(T item);
public struct Nullable<T> where T : struct { /*...*/ }
```
- Als je een duidelijke naam moet gebruiken moet je deze prefixen met 'T', en als een type parameter constraints heeft moet je met de naam aangeven wat de cpmstraints zijn.
voorbeeld:
```cs
public interface ISessionChannel<TSession> where TSession : ISession
{
    TSession Session { get; }
}
```
- Door de code analysis rule 'CA1715' aan te zetten kan je ervoor zorgen dat type parameters de juiste naam krijgen.

# language guidelines
Dit zijn regels voor over hoe je je code moet schrijven.
- gebruik waar mogelijk moderne c# features en c# versies.
- Gebruik zo min mogelijk outdated c# constructs.
- Catch alleen exceptions die je goed kan behandelen.
- Catch nooit de System.Exception zonder een exception filter te gebruiken.
- Gebruik specifieke exception types zodat je duidelijke error messages krijgt.
- Gebruik LINQ queries en methods voor collection manipulatie om de code leesbaarder te maken.
- Als je asynchronus gaat programmeren gebruik je async en await je I/O-bound operations.
- Zorg ervoor dat je geen deadlocks veroorzaakt.
- Gebruik Task.ConfigureAwait waar het logisch is.
- Gebruik de language keywords inplaats van de runtime types(de class variant). Dus voor een string gebruik je string en niet System.String. Dit geld ook voor nint en nuint.
- Gebruik int in plaats van de unsigned types.
- Schijf code met duidelijkheid en simpelheid in gedachte.
- Vermijdt onodig complexe en ingewikelde code logic.

## Strings
- Gebruik string interpolation om korte strings te concatenaten.
voorbeeld:
```cs
string displayName = $"{nameList[n].LastName}, {nameList[n].FirstName}";
```
- Als je strings wilt appenden in loops, voornamelijkt als je met grote strings werkt, moet je een System.Text.StringBuilder object gebruiken.
voorbeeld:
```cs
var phrase = "lalalalalalalalalalalalalalalalalalalalalalalalalalalalalala";
var manyPhrases = new StringBuilder();
for (var i = 0; i < 10000; i++)
{
    manyPhrases.Append(phrase);
}
```
- Geef een voorkeur aan raw string literals over escape sequences of strings waar je escape characters gebruikt.
voorbeeld:
```cs
var message = """
    This is a long message that spans across multiple lines.
    It uses raw string literals. This means we can 
    also include characters like \n and \t without escaping them.
    """;
```
- Gebruik expression based string interpolation(dus bijvoorbeeld $"{x}, {y}") in plaats van position based string interpolation(dus String.Format("{0}, {1}", x, y)).
voorbeeld:
```cs
Console.WriteLine("scoreQuery:");
foreach (var student in scoreQuery)
{
    Console.WriteLine($"{student.Last} Score: {student.score}");
}
```

## constructors and initialization
- Gebruik required properties in plaats van Constructors om propertie value initialization te forceren.
voorbeeld:
```cs
public class LabelledContainer<T>(string label)
{
    public string Label { get; } = label; // bad
    public required T Contents // good
    { 
        get;
        init;
    }
}
```

## Arrays and Collections
- Gebruik collection expression om collection types te initializeren.
voorbeeld:
```cs
string[] vowels = [ "a", "e", "i", "o", "u" ];
```

## Delegates
- Gebruik Func<> en Action<> in plaats van delegate types definieeren. In een class definieer je de delegate method.
voorbeeld:
```cs
Action<string> actionExample1 = x => Console.WriteLine($"x is: {x}");

Action<string, string> actionExample2 = (x, y) =>
    Console.WriteLine($"x is: {x}, y is {y}");

Func<string, int> funcExample1 = x => Convert.ToInt32(x);

Func<int, int, int> funcExample2 = (x, y) => x + y;
```
- Call de method door de signature van de Action<> of Func<> te gebruiken.
voorbeeld:
```cs
actionExample1("string for x");

actionExample2("string for x", "string for y");

Console.WriteLine($"The value is {funcExample1("1")}");

Console.WriteLine($"The sum is {funcExample2(1, 2)}");
```
- Als je een variable van een delegate type definieerd gebruik je de korte syntax voor het creeren van een object.
voorbeeld:
```cs
// Good code
Del exampleDel2 = DelMethod;
exampleDel2("Hey");

// Bad code
Del exampleDel1 = new Del(DelMethod);
exampleDel1("Hey");
```
- In een class definieer je de delegate type en een method met een gelijke method signature.
voorbeeld:
```cs
public delegate void Del(string message);

public static void DelMethod(string str)
{
    Console.WriteLine($"DelMethod argument: {str}");
}
```

## Try-catch en using statements
- Gebruik een try-catch statement voor exception handling.
voorbeeld:
```cs
static double ComputeDistance(double x1, double y1, double x2, double y2)
{
    try
    {
        return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
    catch (System.ArithmeticException ex)
    {
        Console.WriteLine($"Arithmetic overflow or underflow: {ex}");
        throw;
    }
}
```
- Als je een try-finally statement hebt waar alles dat in het finally block staat is het callen van een Dispose method, moet je de korte using syntax gebruiken.
voorbeeld:
```cs
// Bad code 
Font bodyStyle = new Font("Arial", 10.0f);
try
{
    byte charset = bodyStyle.GdiCharSet;
}
finally
{
    bodyStyle?.Dispose();
}

// Good code
using Font normalStyle = new Font("Arial", 10.0f);
byte charset3 = normalStyle.GdiCharSet;
```

## && and || operators
- Gebruik de `&&` en de `||` in plaats van de `&` en de `|` operators wanneer je een comparison uitvoert.

## new operator
- Gebruik een van de korte manieren van een object instantiaten wanneer het variable type gelijk is aan het object type. Dit geld niet als het variable een interface type of de base class van de runtime type.
voorbeelden:
```cs
var firstExample = new ExampleClass();
ExampleClass instance2 = new();
```
- Gebruik object initializers om object creation simpler te maken.
voorbeeld:
```cs
// Good code
var thirdExample = new ExampleClass { Name = "Desktop", ID = 37414,
    Location = "Redmond", Age = 2.3 };

// bad code
var fourthExample = new ExampleClass();
fourthExample.Name = "Desktop";
fourthExample.ID = 37414;
fourthExample.Location = "Redmond";
fourthExample.Age = 2.3;
```

## event handling
- Bij het definieeren van event handlers, die je niet later gaat verwijderen, gebruik je lambda expressions.
voorbeeld:
```cs
public Form2()
{
    this.Click += (s, e) =>
        {
            MessageBox.Show(
                ((MouseEventArgs)e).Location.ToString());
        };
}
```

## Static members
- Call static members met de class name, dus: ClassName.StaticMember. Dit zorgt ervoor dat je code leesbaarder is door duidelijk te maken wat static is.
- Gebruik bij het referencieeren van een static member niet een child class, maar de base class. Dit is misschien goed bij compilatie maar zorgt ervoor dat het onduidelijk is als je in welke class de static member is.
- Gebruik zo min mogelijk verschillende classes met static members.
- Stop alle program wide constants in een static class genaamt Constants.

## LINQ queries
- Gebruik duidelijke en beschrijvende namen voor query variables.
voorbeeld:
```cs
// alle klanten die in seattle zijn.
var seattleCustomers = from customer in Customers
                       where customer.City == "Seattle"
                       select customer.Name;
```
- Gebruik aliasen voor variables in een query zodat de property names in anonymous types correct worden gekapitaliseerd, met PascalCase.
voorbeeld:
```cs
var localDistributors =
    from customer in Customers
    join distributor in Distributors on customer.City equals distributor.City
    select new { Customer = customer, Distributor = distributor };
```
- Geef nieuwe namen aan properties waar de resulterende naam ambitieus zou zijn.
voorbeeld:
```
// good code
var localDistributors2 =
    from customer in Customers
    join distributor in Distributors on customer.City equals distributor.City
    select new { CustomerName = customer.Name, DistributorName = distributor.Name };
// bad code
var localDistributors2 =
    from customer in Customers
    join distributor in Distributors on customer.City equals distributor.City
    select new { customer.Name, distributor.Name };
```
- Gebruik implicit typing(dus het var keyword) voor het datatype van LINQ query en LINQ range variables. Dit moet omdat een LINQ operation vaak anonymous types returned.
- Align query clauses onder de "from" clause.
voorbeeld:
```cs
var seattleCustomers = from customer in Customers
                       where customer.City == "Seattle"
                       select customer.Name;
```
- Gebruik "where" clauses voor andere clauses om ervoor te zorgen dat latere query clauses met minder data hoeven te werken.
- Gebruik voor LINQ queries de method formen en niet de keyword formen.
- Krijg toegang tot innerlijke collecties door middel van nieuwe "from" clauses en niet "join" clauses.
voorbeeld:
```cs
var scoreQuery = from student in students
                 from score in student.Scores
                 where score > 90
                 select new { Last = student.LastName, score };
```

## Implicitly typed local variables
Dit gaat over wanneer je de 'var' en 'dynamic' keywords gebruikt.
- Gebruik implicit typing bij local variables als het type van het variable duidelijk is van de rechter kant van de assignment.
voorbeeld:
```cs
var message = "This is clearly a string.";
var currentTemperature = 27;
```
- Gebruik niet var als het onduidelijk is wat het datatype van het variable is aan de rechter kant van de assignment. Ookal denk je dat de method name het duidelijk maakt.Een variable type is duidelijk als het het new keyword gebruikt, een explicit cast heeft(voorbeeld (int)17f), of een assignment van een literal value is.
voorbeeld:
```cs
var message = "This is clearly a string.";
var currentTemperature = 27;
```
- Gebruik geen variable names om duidelijk te maken wat het datatype van een variable is. Dit kan incorrect zijn.
voorbeeld:
```cs
// bad code
var inputInt = Console.ReadLine();
Console.WriteLine(inputInt);
```
- Gebruik geen 'var' in plaats van dynamic. Gebruik 'dynamic' wanneer je tijdens de runtime wilt bepalen wat het datatype is.
- Gebruik 'var' voor het variable in for loops.
- Gebruik geen 'var' om voor het variable in foreach loops, omdat in de meeste instanties het niet duidelijk is wat het type van de collectie is.

## Namespace decleration and using statements
- Gebruik in files een file scoped namespace decleration.
voorbeeld:
```cs
namespace MySampleCode;
```
- Plaats using statements na de namespace decleration en stop minstens een lege lijn tussen de namespace decleration en de using statements.
voorbeeld:
```cs
namespace MyNamespace;

using System;
```

# Style guidelines
- Je moet als je een variable in een method body maakt deze ook meteen initializeren.
- Als een method maar 1 lijn lang is, schijf je deze als lambda functie. Je plaatst de body van de method op een nieuwe regel, en indenteerd deze een niveau.
voorbeeld:
```cs
// Bad code
public void ExampleMethod()
{
  Console.WriteLine("e");
}

// Good code
public void ExampleMethod() =>
  Console.WriteLine("e");
```
- Geef altijd aan wat de access modifier van een method, field, class, etc.
voorbeeld:
```cs
// Good code
private int x = 1;
public void Start()
{
}
protected string Name { get; private set; }
// Bad code
int x = 1;
void Start()
{
}
string Name { get; private set; }
```
- Een property is altijd public, geef bij de getter en setter aan of deze private is.
- Een property met een body moet je over meerdere lijnen schrijven.
voorbeeld:
```cs
public string Name 
{
  get
  {
    return "";
  }
  set;
}
```
- Een property zonder body moet je op een lijn schrijven, je stops een spatie tussen de get en set keywords en na het openende haakje en voor het sluitende haakje.
voorbeeld:
```cs
public string Name { get; set; }
```
- Gebruik voor indentatie 4 spaties en geen tabs.
- Hou een constante afstand bij je code, om het leesbaarder te maken.
- Limiteer een lijn code tot 65 characters lang.
- Lange code statements moet je breaken om ze leesbaarder te maken. Je plaatst een enter na een segment van die lijn, bij een if statement zou dit na een logical or of logical and operator zijn en bij een keting van method calls zou dat na elke method call zijn.
voorbeeld:
```cs
// Good code
if (true ||
    false &&
    true)
ExampleClass
    .ExampleMethod()
    .ExampleOtherMethod();

// Bad code
if (true || false 
    && true)
ExampleClass.ExampleMethod().ExampleOtherMethod();
```
- In het geval van lange method call ketingen moet je voor elke method call hem op een nieuwe lijn plaatsen.
- Gebruik de "Allman" stijl voor haakjes. Haakjes zijn gelijk aan het huidige indentatie level met hun indentatie.
voorbeeld:
```cs
// Good code
if (true)
{

}
public class ExampleClass
{
  public void ExampleMethod(string argument)
  {
  }
}

// Bad code
if (true) {

}
public class ExampleClass {
  public void ExampleMethod(string argument) {
  }
}
```
- Een enter moet voor een binary operator(bitwise operators) staan.
- Naa elke operator plaatst je een spatie.
voorbeeld:
```cs
// Good code
int x = 15 + 18;

// Bad code 
int x=15+18;
```

## Comment style
- Gebruik single line comments('//') voor korte uitleg van wat een lijn code doet.
- Plaats single line comments('//') boven een lijn code staan.
- Bij een gebroken lijn code, waar je bij de segmenten uitleg wilt geven plaats je de comments achter het segment, en heb je een spatie tussen het laatste karakter op die lijn en de start van de comment(de '//').
voorbeeld:
```cs
// Good code
if (true || // uitleg
    false) // uitleg

// Bad code
if (true ||// uitleg
    false)// uitleg
```
- Bij single line comments plaats je een spatie na de start van de comment, de '//'.
- Voor lange uitleggen gebruik je multi-line comments '/* */'.
- Voor het beschrijven van classes, methods, fields en alle public members gebruik je xml-comments(de '///').
- Voor documentatie comments gebruiken wij [xml-doc](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/).
- Beeindig comment text met een punt.
- Begin comment text met een hoofdletter.

## Layout conventions
- Heb maar een statement per lijn.
- Heb maar een decleratie per lijn.
- Als een continuatie van een lijn niet automatisch word indentet, indent ze dan met 4 spaties.
- Plaats teminste een lege lijn tussen method definitions en property definitions.
- Gebruik haakjes om duidelijk te laten zien welke clauses in expressions bij elkaar horen.
voorbeeld:
```cs
if ((startX > endX) && (startX > previousX))
{
    // Take appropriate action.
}
```

# File and folder structure
- Heb voor alles dat samen is gegroepeerd een eigen folder. Dus voor alle vijanden heb je een enemy folder en voor bijvoorbeeld levels heb je een levels folder.
- Elke class, enum, record, struct, etc, hoe klijn deze ook is, heeft een eigen file.
- Elke base class zit in een subfolder genaamd abstractions, en dan stop je sub classes in parent folder van de abstractions.
- Als een base class een andere base class als sub class krijgt, bijvoorbeeld heb je een base enemy class en de base boss class inherit van de enemy class, stop je deze in een subdirectory van de directory van de base class.
- De naam van directories zijn in PascalCase.
- De naam van een directory zecht wat er in die directory zit.
- Alles dat in een directory zit is suffixed met de naam van die directory, dus als je een Enemy directory hebt hebben alle files erin de suffix 'Enemy'. De uitzondering van deze regel is models.
- De root folder voor de scripts is leeg.
- De root folder voor scripts is de scripts folder.
- Als je 2 classes hebt die gelijke code hebt maak je een base class voor deze classes.
- Je maakt een interface als je wilt definieeren wat een class moet kunnen doen, zonder te zeggen hoe de class dat moet doen.
- Interfaces plaats je in de abstractions subdirectory, deze subdirectory is in de directory waar hij wordt gebruikt.
- Folder namen zijn meervoud.
- Generale interfaces plaats je in een globale abstractions directory.

# Code structure
- Een class is op deze manier geordened, met een lege regel tussen elk onderdeel:
static en constant fields
headers
fields
properties
constructor/awake en start methods
methods
operators
- Tussen elke static en constant field heb je een lege regel.
- Tussen alle properties en methods heb je een lege regel.
- Elke groep order je op access modifier, dit doe je in de volgende volgorde:
private
internal
protected
protected internal
public
- Je ordered de modifiers van fields, methods properties, classes, etc op de volgende manier:
sealed
new
<access modifier>
static
readonly
unsafe
virtual
safe
abstract
volatile
closed
override
async
extern
const
<data type/return value/class keyword/event keyword/etc>

# method body structure
- Tussen elk blok code moet een lege lijn. Een blok code is alle code die ongeveer hetzelfde doet in een method body.
voorbeeld:
```cs
int x = 1;
int y = 6;
int z = x + y;

if (z == 7)
{
  // do something
}

return z;
```
- Als je bij een method call maakt waar door de namen van variables niet duidelijk is wat elk parameter is, moet je dat specifiseren.
voorbeeld:
```cs
// Bad code
Quaternion.Euler(startDistance, 0, 0);

// Good code
Quaternion.Euler(x: startDistance, y: 0, z: 0);
```
- Als je in een method iets optioneel maakt gebruik je geen boolean maar maak je een enum die duidelijk maakt wat er wordt aangezet. Als er al een enum is met dezelfde
waarden hoef je alleen niet een nieuwe enum te maken als de enum even duidelijk zou maken wat de method nodig heeft.
voorbeeld:
```cs
// Bad code
public void DoMath(bool shouldSubstract)
{
}

// Good code
public enum MathOperation
{
  Substract,
  Addition
}

public void DoMath(MathOperation operation)
{
}

```
- Alleen helper classes en constants classes mogen static zijn.

## Magic values
- Gebruik geen magic numbers en magic strings.
- Als je magic numbers of magic strings ergens gebruikt leg je uit wat deze betekenen met een comment.

## When to make methods
- Als een method een blok code duidelijker kan maken door een naam te hebben maak je een method.
- Als je dezelfde code 2 keer herhaald, maakt niet uit of er een waarde verschilt, maak je voor dit blok code een method.

## When to make variables
- Stop zoveel mogelijk values in een variable.
- Sla de resultaten van somen op in een variable voor duidelijkheid.
- Als je een value meerdere keren voor hetzelfde gebruikt maak je er een variable voor.
- Gebruik een variable voor een value wanneer het duidelijker wordt wat de waarde betekent.

# Git
- Iedereen moet een pr goedkeuren.
- Als de code in een pr zig niet aan de code conventies houdt, moet je deze afwijzen.
- Der is een branch per persoon, per feature. Dus bijvoorbeeld "lars/physics".
