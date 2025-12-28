# Custom Recipes System

A szerver-oldali recept testreszabási rendszer, amely lehetővé teszi a szerver adminok számára, hogy módosítsák a crafting recepteket.

## Áttekintés

```
┌─────────────────────────────────────────────────────────────────┐
│  SZERVER                                                        │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ server.cfg                                               │    │
│  │   CustomRecipesEnabled = true                            │    │
│  │                                                          │    │
│  │ recipes.json                                             │    │
│  │   - Módosított receptek listája                          │    │
│  │   - TechType + Ingredients                               │    │
│  └─────────────────────────────────────────────────────────┘    │
│                           │                                      │
│                           ▼                                      │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ InitialPlayerSync                                        │    │
│  │   - Kliens csatlakozáskor megkapja a recepteket          │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│  KLIENS                                                         │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ CustomRecipeInitialSyncProcessor                         │    │
│  │   - Alkalmazza a módosított recepteket                   │    │
│  │   - CraftData.techData felülírása                        │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
```

## Konfiguráció

### server.cfg

```ini
# Custom Recipes bekapcsolása
CustomRecipesEnabled = true
```

### recipes.json

A `recipes.json` fájl a szerver save mappájában található. A fájl tartalmazza az összes módosítható receptet.

#### Formátum

```json
{
  "Recipes": [
    {
      "TechType": "Knife",
      "CraftAmount": 1,
      "Ingredients": [
        { "TechType": "Titanium", "Amount": 2 },
        { "TechType": "Silicone", "Amount": 1 }
      ]
    }
  ]
}
```

#### Mezők

| Mező | Típus | Leírás |
|------|-------|--------|
| `TechType` | string | A craftolható item neve (lásd: TechType lista) |
| `CraftAmount` | int | Hány darab készül egy craftból (alapértelmezett: 1) |
| `Ingredients` | array | Az összetevők listája |
| `Ingredients[].TechType` | string | Az összetevő neve |
| `Ingredients[].Amount` | int | Szükséges mennyiség |

## Használat

1. Állítsd be a `CustomRecipesEnabled = true` értéket a `server.cfg` fájlban
2. Szerkeszd a `recipes.json` fájlt a kívánt receptekkel
3. Indítsd újra a szervert
4. A csatlakozó kliensek automatikusan megkapják a módosított recepteket

## Példák

### Nehezített Survival mód

```json
{
  "Recipes": [
    {
      "TechType": "Knife",
      "CraftAmount": 1,
      "Ingredients": [
        { "TechType": "Titanium", "Amount": 3 },
        { "TechType": "Silicone", "Amount": 2 }
      ]
    },
    {
      "TechType": "Scanner",
      "CraftAmount": 1,
      "Ingredients": [
        { "TechType": "Titanium", "Amount": 3 },
        { "TechType": "Battery", "Amount": 2 },
        { "TechType": "Gold", "Amount": 1 }
      ]
    },
    {
      "TechType": "Seaglide",
      "CraftAmount": 1,
      "Ingredients": [
        { "TechType": "Battery", "Amount": 2 },
        { "TechType": "Lubricant", "Amount": 2 },
        { "TechType": "CopperWire", "Amount": 1 },
        { "TechType": "Titanium", "Amount": 3 }
      ]
    }
  ]
}
```

### Könnyített mód (több item egy craftból)

```json
{
  "Recipes": [
    {
      "TechType": "Titanium",
      "CraftAmount": 4,
      "Ingredients": [
        { "TechType": "ScrapMetal", "Amount": 1 }
      ]
    },
    {
      "TechType": "Battery",
      "CraftAmount": 2,
      "Ingredients": [
        { "TechType": "Copper", "Amount": 1 },
        { "TechType": "AcidMushroom", "Amount": 2 }
      ]
    }
  ]
}
```

## TechType Lista

Az alábbi lista tartalmazza a leggyakrabban használt TechType-okat. A teljes lista megtalálható a Subnautica forráskódjában.

### Alapanyagok (Raw Materials)

| TechType | Magyar név |
|----------|------------|
| `Titanium` | Titán |
| `Copper` | Réz |
| `Gold` | Arany |
| `Silver` | Ezüst |
| `Lead` | Ólom |
| `Diamond` | Gyémánt |
| `Lithium` | Lítium |
| `Magnetite` | Magnetit |
| `Nickel` | Nikkel |
| `Kyanite` | Kyanit |
| `Quartz` | Kvarc |
| `Salt` | Só |
| `Sulphur` | Kén |
| `Ruby` | Rubin |
| `AluminumOxide` | Alumínium-oxid |
| `UraniniteCrystal` | Uraninit kristály |
| `ScrapMetal` | Fémhulladék |
| `CrashPowder` | Robbanópor |

### Növények és Gombák

| TechType | Magyar név |
|----------|------------|
| `AcidMushroom` | Savas gomba |
| `WhiteMushroom` | Fehér gomba |
| `CreepvinePiece` | Kúszónövény darab |
| `CreepvineSeedCluster` | Kúszónövény magcsokor |
| `BloodOil` | Vérolaj |
| `CoralChunk` | Korall darab |
| `JellyPlant` | Zselénövény |
| `TreeMushroomPiece` | Fagomba darab |

### Feldolgozott anyagok (Processed Materials)

| TechType | Magyar név |
|----------|------------|
| `TitaniumIngot` | Titán tömb |
| `PlasteelIngot` | Plastacél tömb |
| `Glass` | Üveg |
| `EnameledGlass` | Zománcozott üveg |
| `Silicone` | Szilikon |
| `FiberMesh` | Szálháló |
| `Lubricant` | Kenőanyag |
| `Bleach` | Fehérítő |
| `Benzene` | Benzol |
| `HydrochloricAcid` | Sósav |
| `Polyaniline` | Polianilin |
| `AramidFibers` | Aramid szálak |
| `Aerogel` | Aerogél |

### Elektronika

| TechType | Magyar név |
|----------|------------|
| `Battery` | Elem |
| `PowerCell` | Energiacella |
| `CopperWire` | Rézvezeték |
| `WiringKit` | Kábelkészlet |
| `ComputerChip` | Számítógép chip |
| `AdvancedWiringKit` | Haladó kábelkészlet |
| `ReactorRod` | Reaktorrúd |

### Eszközök (Tools)

| TechType | Magyar név |
|----------|------------|
| `Knife` | Kés |
| `Scanner` | Szkenner |
| `Flashlight` | Zseblámpa |
| `Welder` | Hegesztő |
| `Builder` | Építő |
| `AirBladder` | Léghólyag |
| `Flare` | Világítófáklya |
| `DiveReel` | Búvárorsó |
| `Seaglide` | Seaglide |
| `LaserCutter` | Lézervágó |
| `StasisRifle` | Stázispuska |
| `PropulsionCannon` | Hajtóágyú |
| `RepulsionCannon` | Taszítóágyú |
| `Gravsphere` | Gravitációs gömb |
| `Beacon` | Jelzőbója |
| `Terraformer` | Terraformáló |
| `PipeSurfaceFloater` | Csőfelszíni úszó |

### Felszerelés (Equipment)

| TechType | Magyar név |
|----------|------------|
| `Tank` | Oxigénpalack |
| `DoubleTank` | Dupla oxigénpalack |
| `PlasteelTank` | Plastacél palack |
| `HighCapacityTank` | Nagy kapacitású palack |
| `Fins` | Uszony |
| `UltraGlideFins` | Ultra siklóuszony |
| `SwimChargeFins` | Töltő uszony |
| `RadiationSuit` | Sugárzás ruha |
| `RadiationHelmet` | Sugárzás sisak |
| `RadiationGloves` | Sugárzás kesztyű |
| `ReinforcedDiveSuit` | Megerősített búvárruha |
| `ReinforcedGloves` | Megerősített kesztyű |
| `Stillsuit` | Desztilláló ruha |
| `FirstAidKit` | Elsősegély készlet |
| `FireExtinguisher` | Tűzoltó készülék |
| `Rebreather` | Visszalélegző |
| `Compass` | Iránytű |
| `Thermometer` | Hőmérő |
| `MapRoomHUDChip` | Térképszoba HUD chip |

### Járművek (Vehicles)

| TechType | Magyar név |
|----------|------------|
| `Seamoth` | Seamoth |
| `Exosuit` | Exoruha (Prawn Suit) |
| `Cyclops` | Cyclops |
| `RocketBase` | Rakéta alap |

### Bázis elemek (Base Pieces)

| TechType | Magyar név |
|----------|------------|
| `BaseRoom` | Szoba |
| `BaseCorridor` | Folyosó |
| `BaseFoundation` | Alap |
| `BaseHatch` | Ajtó |
| `BaseWindow` | Ablak |
| `BaseLadder` | Létra |
| `BaseReinforcement` | Megerősítés |
| `SolarPanel` | Napelem |
| `BaseBioReactor` | Bioreaktor |
| `BaseNuclearReactor` | Atomreaktor |
| `ThermalPlant` | Hőerőmű |
| `BaseFiltrationMachine` | Szűrőgép |
| `BaseWaterPark` | Vízipark |
| `Fabricator` | Gyártó |
| `Workbench` | Munkapad |
| `BatteryCharger` | Elemtöltő |
| `PowerCellCharger` | CellaTöltő |
| `Locker` | Szekrény |
| `SmallLocker` | Kis szekrény |
| `Bed1` | Ágy |
| `Aquarium` | Akvárium |
| `PlanterBox` | Ültető láda |
| `PlanterPot` | Ültető cserép |
| `PlanterShelf` | Ültető polc |

## Megjegyzések

- A szerver újraindítása szükséges a `recipes.json` módosítások érvényesítéséhez
- Ha egy TechType nem létezik, a szerver figyelmeztető üzenetet ír a logba
- A kliensek automatikusan szinkronizálódnak csatlakozáskor
- A módosítások csak a Nitrox multiplayer session-re vonatkoznak, a single player nem érintett
