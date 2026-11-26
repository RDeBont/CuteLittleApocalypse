# Cute Little Apocalypse — ticketlijst

Doel: feature complete aan het eind van sprint 5, sprint 6 als buffer. De sprints daarna zijn voor feedback, testverslag, polish en oplevering.

Team: Rowan (R) en Thomas (Th). Tycho (T) was lid van het team tot en met week 7 (28-10-2026).

Statuskolom bijhouden: todo / bezig / review / klaar / geschrapt.

## Teamwijziging oktober 2026

Tycho is in week 7 gestopt met de opleiding. Wat hij al gebouwd heeft blijft in de game en blijft in dit bord op zijn naam staan (T). Zijn openstaande tickets zijn verdeeld over Rowan en Thomas. Om met twee personen een complete, speelbare game op te leveren is de scope verkleind. De kern blijft volledig: lopen, springen, schieten met beperkte munitie, beide pony's, checkpoints, menu's, geluid, opslag en de Chinook als einde.

**Herverdeling openstaande tickets van Tycho**

| Ticket | Was | Wordt | Toelichting |
| --- | --- | --- | --- |
| CLA-36 LevelManager | T | Th | Staat in review, Thomas reviewt en merget |
| CLA-40 MenuManager | T | R | Staat in review, Rowan reviewt en merget (hoort bij de UI) |
| CLA-46 SaveManager | T | R | Mogelijk al af: controleren wat er staat, anders afmaken in de kleinere versie |
| CLA-58 Standalone build | T | R | |
| CLA-61 Unit tests | T | R | CLA-61 en CLA-62 samengevoegd tot één ticket |
| CLA-88 Shippable build | T | R | |
| CLA-91 Source code en Git controleren | T | R | |
| CLA-D3 Wekelijkse build en tag | wisselend | R | |

**Geschrapt**

| Wat | Ticket | Reden |
| --- | --- | --- |
| Health-pickups en healing | — | Niet nodig: HP komt terug bij respawn op een checkpoint |
| Brandhaard | CLA-51 (deel) | Extra hazard, de pony's leveren genoeg uitdaging |
| Zone 4 | CLA-52 | Samengevoegd met zone 3: het level heeft drie zones |
| Knop Verder spelen | CLA-40 / CLA-46 (deel) | Eén level, dus niets om verder te spelen |
| Eigen muiscursor | CLA-47 | Er wordt niet met de muis gericht |
| Animatie van de rookpluimen | CLA-54 (deel) | Rook staat stil als decor |
| Eindtekstscherm als los scherm | CLA-56 (deel) | De eindtekst komt op het scherm Level voltooid |
| Vrolijke eindmelodie | CLA-45 (deel) | Alleen de ambient loop |
| Geluid voetstappen en landen | CLA-44 (deel) | Kleine geluiden die weinig toevoegen |
| Aparte unit-test-tickets | CLA-62 | Samengevoegd met CLA-61 |
| Extra levels en extra pony-type | — | Stonden al als extra, nu definitief geschrapt |

## Controletabel: elke sprint een deliverable

| Sprint | Datum | Wat de school verwacht | Waar dat in dit plan zit |
| --- | --- | --- | --- |
| Vooraf |  | Duidelijke richting en plan | Checklist kick-off |
| 1 |  | Basis documentatie en eerste prototype | CLA-09b (MDD), CLA-09c, lopende speler |
| 2 |  | Eerste werkende functionaliteiten | Schieten, vijanden, schade |
| 3 |  | Speelbare versie (intern) | CLA-30 intern testen |
| 4 |  | Demo 1, feedback en verbeterpunten | CLA-31, CLA-31b |
| 5 |  | Klantreview 1 afgerond, productie compleet | CLA-50 t/m CLA-58, feature complete |
| 6 |  | Bijna complete versie en eerste tests | CLA-61, CLA-64 |
| 7 |  | Demo 2, laatste verbeterpunten | CLA-80, CLA-81, CLA-82 |
| 8 |  | Klantreview 2 afgerond | CLA-85 |
| 9 |  | Testverslag afgerond en shippable product | CLA-86, CLA-87, CLA-88 |
| 10 |  | Complete oplevermap | CLA-89 t/m CLA-93 |
| 11 |  | Oplevering en definitieve afronding | CLA-94, CLA-95 |

De datumkolom vullen we zelf in zodra we de sprints op de schoolplanning leggen.

## Basisregel: elke sprint is er iets te laten zien

Op school zijn er vaste momenten waarop iets af moet zijn: Demo 1, Klantreview 1, Demo 2, Klantreview 2 en twee oplevermomenten. Om nooit met lege handen te staan gelden drie afspraken:

1. **main is altijd speelbaar.** Nooit half werk naar main mergen. Werk op een feature-branch en merge pas als het draait.
2. **Elke week een build.** Windows-build exporteren, in de map Builds zetten met de datum in de naam, en de commit taggen. Dat is je vangnet: gaat er iets stuk vlak voor een demo, dan laat je de build van vorige week zien.
3. **Documentatie loopt mee, niet achteraf.** Elke sprint sluit af met het bijwerken van GDD, MDD en logboek. Op de check-ins wordt hier expliciet naar gekeken.

## Doorlopende tickets

Deze lopen de hele periode door en hebben geen einddatum.

**CLA-D1 — Logboek bijhouden** · R, Th  
Datum, ticket-taak, korte beschrijving. Iedereen houdt zijn eigen logboek bij. Wordt gecontroleerd op de check-ins. Het logboek van Tycho loopt tot week 7.

**CLA-D2 — Git-hygiëne** · R, Th  
Kleine commits met duidelijke berichten, werken op feature-branches.

**CLA-D3 — Wekelijkse build en tag** · R (was: wisselend)  
Zie basisregel 2 hierboven.

**CLA-D4 — Bewijsstukken verzamelen** · R, Th  
Per exameneis een screenshot of codefragment vastleggen zodra het onderdeel af is. Niet bewaren tot het einde.

## Vooraf — kick-off

- Groepje gevormd, Unity gekozen en idee gepitcht
- Repo aangemaakt (CLA-01)
- Rollen verdeeld en vastgelegd in hoofdstuk 9.1 van het GDD
- Planning gemaakt (dit document)

Deliverable: duidelijke richting en plan.

## Sprint 1 — opzet en een lopende speler

Doel: er staat een soldaat die over daken loopt en springt. Nog geen animaties, gewoon één sprite.

**CLA-01 — Repo en projectstructuur** · R · klaar  
Git-repo met de officiële Unity .gitignore. Branch main beschermd, iedereen op een eigen feature-branch.

**CLA-02 — Unity-project aanmaken** · R · klaar  
Unity 2D-project, versie vastleggen in de README. Iedereen installeert exact dezelfde versie.

**CLA-03 — Mappenstructuur** · T · klaar  
Sprites, Animations, Prefabs, Scripts, Scenes, UI, Tiles, Audio.

**CLA-04 — Assets importeren en importinstellingen** · R · klaar  
Pixels Per Unit 16, Filter Mode Point, Compression None. De _x4-bestanden niet gebruiken.

**CLA-05 — Sheets slicen** · R · klaar  
Soldaat 32×32, pony's 32×32, tileset 16×16, objecten 16×16, chinook 96×48, knoppen 80×24, HUD 16×16, healthbar 48×8. UI-schermen blijven Single.

**CLA-06 — Scenes, camera en sorting layers** · T · klaar  
MainMenu en Level_01 in Build Settings. Camera Orthographic, Size 5, anti-aliasing uit. Lagen: Background, Decoration, Ground, Enemy, Player, Foreground, UI.

**CLA-08 — Logboek en ticketbord opzetten** · Th · klaar  
Bord aanmaken met deze tickets. Logboek-template voor het hele team.

**CLA-09 — IDE opzetten** · Th · klaar  
Iedereen dezelfde editor-configuratie: Visual Studio of Rider gekoppeld aan Unity, plus een .editorconfig in de repo zodat inspringen en naamgeving overal gelijk zijn.

**CLA-09b — MDD opzetten** · R · klaar  
Multimedia Design Document aanmaken naast het GDD: overzicht van alle art-, audio- en UI-assets met bestandsnaam, formaat, status en licentie.

**CLA-10 — PlayerController: lopen** · T · klaar  
A/D met Rigidbody2D, snelheid 6 units per seconde, sprite spiegelen met flipX.

**CLA-11 — PlayerController: springen** · T · klaar  
Spatie, sprongkracht 12, geen dubbele sprong. Grond-detectie met Physics2D.OverlapCircle en een LayerMask Ground.

**CLA-14 — Tile Palette** · Th · klaar  
RooftopPalette vanuit de geslicete tileset, opslaan in Assets/Tiles.

**CLA-15 — Tilemap-lagen in Level_01** · Th · klaar  
Background, Decoration, Ground en Foreground onder één Grid, elk met een eigen Order in Layer.

**CLA-16 — Ground collider** · Th · klaar  
Tilemap Collider 2D met Composite Collider 2D, Rigidbody2D op Static, Used By Composite aan. Alleen Ground krijgt een collider.

**CLA-17 — Ruw testlevel** · Th · klaar  
Genoeg daken en gaten om beweging en straks gevechten te testen. Nog niet mooi.

**CLA-18 — Camera volgt speler** · T · klaar  
Horizontaal volgen met lichte demping, verticaal beperkt zodat de daklijn in beeld blijft.

**CLA-09c — Documentatie bijwerken** · R · klaar  
GDD en MDD aanvullen met wat er deze sprint besloten en gebouwd is.

Deliverable: basis documentatie (GDD en MDD) en een eerste prototype dat loopt en springt.

## Sprint 2 — schieten en vijanden

**CLA-20 — WeaponController** · T · klaar  
Semi-automatisch op linkermuisknop, magazijn 7, start 14, maximum 21, herladen met R in 1,2 seconde. Horizontaal schieten in de kijkrichting.

**CLA-21 — Projectile** · T · klaar  
Losse GameObjects met korte levensduur, verdwijnen bij een vijand of muur, 1 schade. Eigen collision layer zodat de speler zichzelf niet raakt.

**CLA-23 — EnemyBase** · Th · klaar  
Hitpoints, schade ontvangen, doodgaan. Beide pony's erven hiervan.

**CLA-24 — Roze pony (EnemyPatrol)** · Th · klaar  
1 HP, patrouilleert over één dakvlak, draait om bij een dakrand, kan niet springen en valt niet van het dak.

**CLA-25 — Paarse pony (EnemyChase)** · Th · klaar  
2 HP, staat stil tot de speler binnen 6 tiles komt, valt dan aan, sneller dan de roze.

**CLA-28 — PlayerHealth** · T · klaar  
3 HP, 1 schade per pony-contact, 1 seconde onschendbaarheid met een knipperende sprite. Geen healing: HP komt alleen terug bij respawn.

**CLA-29 — HUD basis** · R · klaar  
Drie HP-iconen waarbij een leeg icoon #C93B3B wordt, en een munitieteller in de vorm 07 / 14 in #F2B138 die rood wordt bij nul.

**CLA-33 — Munitie-pickup** · R · klaar  
Trigger-collider, plus 7 kogels tot maximaal 21, verdwijnt na oppakken. Als prefab opslaan.

**CLA-29b — Documentatie aanvullen** · R · klaar  
GDD en MDD bijwerken met de gebouwde mechanics.

Deliverable: eerste werkende functionaliteiten.

## Sprint 3 — animaties, polish en demo klaarmaken

**CLA-12 — Animaties speler** · R · klaar  
Idle, Walk met vier frames, Jump, Fall, Shoot en Reload.

**CLA-13 — Animator states en transitions** · R · klaar  
Parameters Speed (float), IsGrounded (bool), Shoot (trigger) en Reload (trigger).

**CLA-22 — Muzzle flash** · R · klaar  
Schietframe met flash op het moment van het schot.

**CLA-26 — Animaties pony's** · Th · todo  
Per pony Walk met vier frames, Idle en Attack. Als prefab opslaan.

**CLA-27 — Detectie-indicator** · Th · klaar  
Uitroepteken boven de pony bij detectie, kort zichtbaar voor de aanval.

**CLA-32 — Checkpoints** · T · klaar  
Checkpoint-script met lichtbaken. Bij verlies terug naar het laatste checkpoint, waarbij munitie, positie en HP worden teruggezet.

**CLA-35 — Val buiten het level** · T · klaar  
Onzichtbare trigger onder het level: 1 HP schade en respawn op het checkpoint.

**CLA-30 — Intern testrondje** · Th · todo  
Spelen en bugs in het bord zetten.

**CLA-31 — Demo 1 voorbereiden** · R, T, Th · klaar  
Build draaien, de doorloop oefenen.

**CLA-30b — MDD en GDD bijwerken** · R · klaar  
Assetlijst en mechanics-hoofdstukken gelijktrekken met wat er echt in de game zit.

Deliverable: speelbare versie (intern), bugs verzameld, MDD en GDD bijgewerkt.

## Sprint 4 — systemen rond de gameplay

**CLA-31b — Feedback Demo 1 verwerken** · R, T, Th · klaar  
Punten uit de demo omzetten in tickets en de belangrijkste meteen oppakken.

**CLA-36 — LevelManager** · T → Th · review  
Level starten, voltooien, game over, en de statistieken bijhouden: tijd, resterende munitie en uitgeschakelde pony's. Gebouwd door Tycho, Thomas reviewt en merget.

**CLA-37 — Chinook en eindtrigger** · R · klaar  
Rotor-animatie van vier frames in een loop, frame met open laadklep bij aankomst. Trigger op het laatste dak die het level voltooit.

**CLA-38 — Canvas en UI-schermen** · R · klaar  
Canvas op Scale With Screen Size, Reference Resolution 320×180, Match 0,5. Panels voor MainMenu, Pause, GameOver en LevelComplete.

**CLA-40 — MenuManager** · T → R · review  
Nieuw spel, Instellingen en Afsluiten (Verder spelen geschrapt). Pauzeren met Esc via Time.timeScale. Klaar als Time.timeScale na hervatten of herstarten gegarandeerd terug op 1 staat. Gebouwd door Tycho, Rowan reviewt, haalt de knop Verder spelen weg en merget.

**CLA-41 — Game over en level voltooid** · R · klaar  
Game over met Opnieuw proberen en Hoofdmenu. Level voltooid met tijd, resterende munitie, uitgeschakelde pony's, een korte eindtekst, Opnieuw en Hoofdmenu.

**CLA-43 — AudioManager** · Th · todo  
Eén AudioMixer met een groep voor muziek en een voor effecten, effecten ongeveer 6 dB boven de muziek. Volume-instellingen vanuit het instellingenscherm.

**CLA-44 — Geluidseffecten inbouwen** · Th · todo  
Schot, herladen, lege klik, sprong, pony-detectie, pony neer, schade, pickup en Chinook. Voetstappen en landing geschrapt. Alleen vrije licenties, bronnen noteren in het MDD.

**CLA-45 — Ambient muziek** · Th · todo  
Eén ambient loop in het level. De eindmelodie is geschrapt.

**CLA-46 — SaveManager** · T → R · klaar  
JSON in de persistent data-map met de volume-instellingen en de beste tijd. Eerst nagaan wat Tycho al gebouwd heeft (staat nog niet op Develop); anders afmaken in deze kleinere versie. Klaar als afsluiten en opnieuw opstarten de instellingen en beste tijd behoudt.

**CLA-47 — Cursor** · R · geschrapt  
Er wordt niet met de muis gericht, de standaardcursor volstaat.

Deliverable: verbeterde versie met werkende menu's, geluid en opslag.

## Sprint 5 — level afmaken en feature complete

**CLA-50 — Zone 2 bouwen** · R · klaar  
Van checkpoint 2 tot checkpoint 3 (x 58 tot 102): twee roze pony's en één paarse pony, één munitie-pickup, één gat om over te springen en een hoog gebouw tussen x 76 en 90.

**CLA-51 — Zone 3 bouwen (inclusief evacuatie)** · R · klaar  
Van checkpoint 3 tot het einde (x 102 tot 169): één roze en twee paarse pony's, één munitie-pickup, drie korte sprongen achter elkaar (x 108 tot 118), een extra checkpoint op x 131 en de Chinook op het laatste dak (x 165). Geen brandhaard.

**CLA-52 — Zone 4 bouwen** · Th · geschrapt  
Samengevoegd met CLA-51.

**CLA-53 — Parallax-achtergrond** · R · klaar  
Twee of drie lagen gebouwen in donkerdere tinten die trager meescrollen.

**CLA-54 — Decoratie plaatsen** · R · todo  
Olievat, kratten, zandzakken, satellietschotel, puin, antennemast en rookpluimen (stilstaand) verdelen over de drie zones.

**CLA-55 — Balanceren** · R, Th · todo  
Loopsnelheid, sprongkracht, munitieverdeling en pony-plaatsing bijstellen na het spelen. Speelduur richting drie à vijf minuten bij een geslaagde run.

**CLA-56 — Begintekstscherm** · R · klaar  
Kort tekstscherm aan het begin van het level. De eindtekst staat op Level voltooid (CLA-41).

**CLA-57 — Klantreview 1** · R, Th · klaar  
Review uitgevoerd met een externe tester (cijfer 7/10) en vastgelegd in het klantreview-verslag. De verbeterpunten staan in CLA-57b.

**CLA-57b — Feedback klantreview 1 verwerken** · R, Th · bezig  
Verbeterpunten omgezet in tickets (CLA-66 t/m CLA-70), GDD (1.3) en MDD (0.4) bijgewerkt. De wens voor meer levels blijft geschrapt (zie Teamwijziging oktober 2026).

**CLA-58 — Standalone build maken** · T → R · todo  
Windows-build exporteren en op een andere computer testen. Klaar als de build start en uit te spelen is zonder Unity erbij.

**CLA-59 — Teamwijziging verwerken** · R · klaar  
Tycho gestopt: tickets herverdeeld, scope verkleind, GDD (1.1), MDD (0.2) en deze ticketlijst bijgewerkt.

Deliverable: klantreview 1 afgerond en het product is feature complete. Alles uit het GDD zit erin en het level is uitspeelbaar.

## Sprint 6 — buffer, tests en kwaliteit

**CLA-60 — Bugfix-ronde** · R, Th  
Alle open bugs uit het bord oplossen.

**CLA-61 — Unit tests munitie- en schadelogica** · T → R  
Unity Test Framework: magazijn, herladen, het maximum van 21, schieten zonder munitie, schade aan speler en pony's, het onschendbaarheidsvenster en doodgaan bij 0 HP.

**CLA-62 — Unit tests schadelogica** · geschrapt  
Samengevoegd met CLA-61.

**CLA-64 — Testverslag opzetten** · Th  
Vaste testgevallen: beweging, springen, schieten, herladen, schade, respawn, opslaan en laden van instellingen, en het voltooien van het level.

**CLA-65 — GDD bijwerken** · R  
Hoofdstuk 9 en de bijlagen bijwerken.

**CLA-66 — Checkpoint duidelijker: vlag** · R · todo  
Uit klantreview 1. Het lichtbaken vervangen door een vlag-sprite in de projectstijl (palet GDD 5.2, outline #12141F, PPU 16): inactief grijs en laag, omhoog bij activatie (4 frames) en wapperend als loop (4 frames). Animator Inactive → Raising → Active en kort de tekst Checkpoint! bij activatie. Klaar als een tester zonder uitleg ziet waar het checkpoint staat en wanneer het geactiveerd is.

**CLA-67 — Sprong aanpassen** · R · todo  
Uit klantreview 1. Variabele spronghoogte (kort indrukken is een lage sprong), snellere val met een maximale valsnelheid, coyote time en jump buffering van ongeveer 0,1 seconde. Alle waarden instelbaar in de Inspector. Klaar als alle sprongen in de drie zones haalbaar blijven en de sprong prettig aanvoelt bij een nieuwe tester.

**CLA-68 — Sorting layers gelijktrekken** · R · todo  
Uit klantreview 1. Alle SpriteRenderers, tilemaps en effecten in Level_01 en de prefabs nalopen op de lagen uit CLA-06. Decor, pickups en checkpoints staan achter de speler, alleen Foreground staat ervoor. Geen posities of colliders aanpassen. Klaar als in alle drie de zones duidelijk is wat voor en wat achter de speler staat.

**CLA-69 — Munitie als kogels in de HUD** · R · todo  
Uit klantreview 1. De teller 07 / 14 vervangen door zeven kogel-iconen voor het magazijn: vol in #F2B138, verbruikt ontzadigd, kort knipperen in #C93B3B bij nul. De reserve blijft als klein getal naast het munitie-icoon. Bijwerken via een event vanuit de WeaponController. Klaar als schieten, herladen en een pickup direct zichtbaar zijn in de HUD.

**CLA-70 — Speler draait niet om (bug)** · R · todo  
Uit klantreview 1: de sprite kijkt niet altijd de looprichting op. De flipX uit CLA-10 controleren, ook tijdens schieten, herladen en springen, en kogels mee laten gaan in de kijkrichting. Klaar als de speler in elke animatie de kant op kijkt waar hij heen loopt.

Deliverable: bijna complete versie en de eerste formele tests.

## Sprint 7 — Demo 2

**CLA-80 — Exameneisen-checklist invullen** · R, Th  
Alle exameneisen naast de game leggen en per eis noteren waar het bewijs zit. Wat ontbreekt wordt meteen een ticket.

**CLA-81 — Demo 2 geven** · R, Th  
Bijna compleet product demonstreren en feedback ophalen.

**CLA-82 — Feedback Demo 2 verwerken** · R, Th  
Punten omzetten in tickets en prioriteren op wat nodig is voor de oplevering.

## Sprint 8 — laatste ontwikkelronde

**CLA-85 — Laatste bugs oplossen** · R, Th  
Alles wat nog open staat in het bord. Vanaf hier geen nieuwe functionaliteit meer.

## Sprint 9 — testen en polish

**CLA-86 — Minimaal drie testrondes uitvoeren** · R, Th  
De vaste testgevallen uit CLA-64 drie keer volledig doorlopen, met de bevindingen per ronde vastgelegd.

**CLA-87 — Testverslag afronden** · Th  
Opzet, testgevallen, resultaten per ronde, en de opgeloste en openstaande bevindingen.

**CLA-88 — Shippable build** · T → R  
Definitieve Windows-build, getest op een computer zonder Unity.

## Sprint 10 — opleverpakket

**CLA-89 — Documentatie compleet maken** · R  
GDD en MDD definitief, inclusief hoofdstuk 9 en de bijlagen.

**CLA-90 — Logboeken controleren** · R, Th  
Logboeken van Rowan en Thomas compleet, plus het logboek van Tycho tot week 7.

**CLA-91 — Source code en Git-commits controleren** · T → R  
Repo opgeruimd, README compleet (team bijwerken) en een commit-geschiedenis die netjes leesbaar is.

**CLA-93 — Presentatie maken** · R, Th  
Verhaal, demo-doorloop en wie wat vertelt. Minstens één keer oefenen met de tijd erbij.

## Sprint 11 — oplevering

**CLA-94 — Oplevering 1** · R, Th  
Product en bewijsstukken inleveren, presentatie geven en aantonen dat aan de exameneisen is voldaan.

**CLA-95 — Oplevering 2** · R, Th  
Presentatie en assessment, plus de laatste ontbrekende onderdelen afronden.
