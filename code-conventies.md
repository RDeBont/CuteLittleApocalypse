# Codeconventies — Unity (C#)

Project: 2D-platformer
Team: 3 personen
Engine: Unity | Taal: C# | Versiebeheer: Git

Iedereen houdt zich aan dit document. Bij twijfel: kijk hoe de bestaande code het doet.

Alles in dit project is Nederlandstalig: klassenamen, variabelen, methodes, commentaar en commitberichten.

---

## 1. Naamgeving

| Type | Stijl | Voorbeeld |
|---|---|---|
| Klasse / struct / enum | PascalCase | `SpelerBesturing`, `VijandType` |
| Methode | PascalCase | `NeemSchade()` |
| Publiek veld / property | PascalCase | `public int Leven { get; private set; }` |
| Privéveld | camelCase | `loopSnelheid` |
| Geserialiseerd privéveld | camelCase | `[SerializeField] private float springKracht;` |
| Constante | PascalCase | `const float MaxSnelheid = 10f;` |
| Parameter / lokale variabele | camelCase | `float verstrekenTijd` |
| Interface | I + PascalCase | `IBeschadigbaar` |
| Bestandsnaam | gelijk aan klassenaam | `SpelerBesturing.cs` |

Regels:
- Geen afkortingen: `vijandSpawnPunt`, niet `vspwn`.
- Geen Hongaarse notatie (`m_`, `_`, `str`).
- Booleans lezen als een vraag: `staatOpGrond`, `magSpringen`, `heeftSleutel`.
- Unity-methodes (`Start`, `Update`, `Awake`) houden hun Engelse naam — die zijn vastgelegd door de engine.

## 2. Bestands- en mapstructuur

```
Assets/
  Scripts/
    Speler/
    Vijanden/
    UI/
    Managers/
    Hulpmiddelen/
  Prefabs/
  Scenes/
  Sprites/
  Audio/
  Animaties/
```

- Eén klasse per bestand.
- De scriptmap volgt het onderdeel van het spel, niet het bestandstype.

## 3. Opmaak

- Inspringen: 4 spaties, geen tabs.
- Accolades altijd op een nieuwe regel (Allman-stijl).
- Accolades ook bij regels van één statement.
- Maximaal ongeveer 120 tekens per regel.
- Eén lege regel tussen methodes.

```csharp
if (staatOpGrond)
{
    Spring();
}
```

## 4. Volgorde binnen een klasse

1. Constanten
2. Geserialiseerde velden
3. Privévelden
4. Properties
5. Unity-methodes (`Awake`, `Start`, `Update`, `FixedUpdate`, `OnCollisionEnter2D`)
6. Publieke methodes
7. Privémethodes

## 5. Unity-specifiek

- Gebruik `[SerializeField] private` in plaats van `public` om iets in de Inspector te tonen.
- Sla componenten op in `Awake()`, roep nooit `GetComponent()` aan in `Update()`.
- Physics (`Rigidbody2D`, krachten) hoort in `FixedUpdate()`, invoer in `Update()`.
- Vermenigvuldig beweging altijd met `Time.deltaTime`.
- Vermijd `GameObject.Find()` en `SendMessage()`; gebruik referenties of events.
- Gebruik `CompareTag("Vijand")` in plaats van `gameObject.tag == "Vijand"`.
- Losse getallen horen in een `[SerializeField]` of `const`, niet hardcoded in de logica.
- Zet `[RequireComponent(typeof(Rigidbody2D))]` boven klassen die daarvan afhankelijk zijn.

```csharp
[RequireComponent(typeof(Rigidbody2D))]
public class SpelerBesturing : MonoBehaviour
{
    [SerializeField] private float loopSnelheid = 5f;

    private Rigidbody2D rigidbody2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
}
```

## 6. Commentaar

- Commentaar legt uit *waarom* iets gebeurt, niet *wat*. De code zegt al wat er gebeurt.
- Zet een samenvatting boven publieke methodes die niet vanzelfsprekend zijn:

```csharp
/// <summary>
/// Brengt schade toe en start de onkwetsbaarheidstimer.
/// </summary>
public void NeemSchade(int hoeveelheid)
```

- Geen uitgecommentarieerde code committen; Git bewaart de historie al.
- `// TODO:` mag, maar met naam erbij: `// TODO (Rowan): geluid toevoegen`.

## 7. Methodes en klassen

- Een methode doet één ding. Boven ongeveer 30 regels: opsplitsen.
- Een klasse heeft één verantwoordelijkheid (`SpelerBeweging` en `SpelerLeven` apart).
- Maximaal 3 à 4 parameters; meer betekent meestal dat er een klasse ontbreekt.
- Nest niet dieper dan 3 niveaus; gebruik vroege returns.

## 8. Git

- Branches: `main` (altijd werkend) en `feature/<naam>`, `fix/<naam>`.
- Nooit direct naar `main` pushen; werk via pull requests.
- Commitberichten in het Nederlands en in de gebiedende wijs:
  - `voeg sprong van speler toe`
  - `los vijandbeweging op hellingen op`
  - niet: `dingen`, `update`, `asdf`
- Kleine, logische commits. Niet één commit per dag met alles erin.
- Gebruik de Unity-`.gitignore` (`Library/`, `Temp/`, `Build/` niet committen).
- Zet Git LFS aan voor grote assets.

## 9. Foutafhandeling

- Controleer referenties die leeg kunnen zijn voordat je ze gebruikt.
- Gebruik `Debug.LogWarning()` en `Debug.LogError()` bij onverwachte situaties.
- Verwijder debugmeldingen voor de oplevering, of zet ze achter een debugvlag.

## 10. Controlelijst voor review

Voor je een pull request opent:

- [ ] De code compileert zonder waarschuwingen.
- [ ] De naamgeving volgt dit document.
- [ ] Geen uitgecommentarieerde of dode code.
- [ ] Geen overbodige `Debug.Log`-regels.
- [ ] Getest in de editor.
- [ ] Commitberichten zijn duidelijk.
