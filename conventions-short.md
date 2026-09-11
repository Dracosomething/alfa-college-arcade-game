## PascalCase
Dit gaat over waar je PascalCase moet gebruiken voor een naam.
- gebruik PascalCase wanneer je een class, interface, struct of delegate een naam geeft.
voorbeelden:
- Wanneer je een interface een naam geeft gebruik je PascalCase en prefix je de naam met een 'I'.
voorbeeld:
- Wanneer je een naam geeft aan public members van types, zoals fields, properties, events, gebruik je PascalCase. Gebruik ook PascalCase voor alle methods en functions.
voorbeeld:
- Bij het maken van een record moet je PascalCase gebruiken voor de parameters omdat deze public zijn.
voorbeeld:
- Gebruik PascalCase voor alle constants, ongeacht de access modifier.
voorbeeld:

## camelCase
Dit gaat over waar je camelCase moet gebruiken voor een naam en wat extra regels voor naamgeving.
- Gebruik camelCase bij het geven van een naam aan private of internal non-constant fields en prefix ze met een underscore.
- Gebruik camelCase voor de namen van local variables, inclusief instances van een delegate type.
- Static fields die private of internal zijn prefix je met 's_' en thread static fields prefix je met 't_'.
- Voor method parameters moet je camelCase gebruiken, dit geld ook voor parameters van constructors.
- Bij primary constructor parameters voor classes en structs gebruik je camelCase, omdat dit constant is met andere method parameters.

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
