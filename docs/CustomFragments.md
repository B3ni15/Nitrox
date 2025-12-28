# Custom Fragments System

A szerver-oldali fragment testreszabási rendszer, amely lehetővé teszi a szerver adminok számára, hogy módosítsák, hány fragment szkennelésére van szükség egy TechType feloldásához.

## Áttekintés

```
┌─────────────────────────────────────────────────────────────────┐
│  SZERVER                                                        │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ server.cfg                                               │    │
│  │   CustomFragmentsEnabled = true                          │    │
│  │                                                          │    │
│  │ fragments.json                                           │    │
│  │   - Módosított fragment követelmények                    │    │
│  │   - TechType + TotalFragments                            │    │
│  └─────────────────────────────────────────────────────────┘    │
│                           │                                      │
│                           ▼                                      │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ InitialPlayerSync                                        │    │
│  │   - Kliens csatlakozáskor megkapja a fragmenteket        │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│  KLIENS                                                         │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ CustomFragmentInitialSyncProcessor                       │    │
│  │   - Alkalmazza a módosított fragment számokat            │    │
│  │   - PDAScanner.GetEntryData felülírása                   │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
```

## Konfiguráció

### server.cfg

```ini
# Custom Fragments bekapcsolása
CustomFragmentsEnabled = true
```

### fragments.json

A `fragments.json` fájl a szerver save mappájában található. A fájl tartalmazza az összes módosítható fragment követelményt.

#### Formátum

```json
{
  "Fragments": [
    {
      "TechType": "Seaglide",
      "TotalFragments": 4
    },
    {
      "TechType": "Seamoth",
      "TotalFragments": 5
    }
  ]
}
```

#### Mezők

| Mező | Típus | Leírás |
|------|-------|--------|
| `TechType` | string | A feloldható TechType neve (pl. "Seamoth", "Seaglide") |
| `TotalFragments` | int | Hány fragment szkennelése szükséges a feloldáshoz |

## Használat

1. Állítsd be a `CustomFragmentsEnabled = true` értéket a `server.cfg` fájlban
2. Szerkeszd a `fragments.json` fájlt a kívánt fragment követelményekkel
3. Indítsd újra a szervert
4. A csatlakozó kliensek automatikusan megkapják a módosított követelményeket

## Példák

### Nehezített Survival mód (több fragment szükséges)

```json
{
  "Fragments": [
    { "TechType": "Seaglide", "TotalFragments": 5 },
    { "TechType": "Seamoth", "TotalFragments": 8 },
    { "TechType": "Cyclops", "TotalFragments": 12 },
    { "TechType": "Exosuit", "TotalFragments": 10 },
    { "TechType": "Constructor", "TotalFragments": 6 },
    { "TechType": "Beacon", "TotalFragments": 4 },
    { "TechType": "Gravsphere", "TotalFragments": 4 },
    { "TechType": "LaserCutter", "TotalFragments": 5 },
    { "TechType": "StasisRifle", "TotalFragments": 5 },
    { "TechType": "PropulsionCannon", "TotalFragments": 5 },
    { "TechType": "Workbench", "TotalFragments": 6 },
    { "TechType": "BaseNuclearReactor", "TotalFragments": 6 },
    { "TechType": "ThermalPlant", "TotalFragments": 5 },
    { "TechType": "BaseMapRoom", "TotalFragments": 5 },
    { "TechType": "BaseMoonpool", "TotalFragments": 6 }
  ]
}
```

### Könnyített mód (kevesebb fragment)

```json
{
  "Fragments": [
    { "TechType": "Seamoth", "TotalFragments": 2 },
    { "TechType": "Cyclops", "TotalFragments": 3 },
    { "TechType": "Exosuit", "TotalFragments": 2 },
    { "TechType": "LaserCutter", "TotalFragments": 1 },
    { "TechType": "StasisRifle", "TotalFragments": 1 }
  ]
}
```

## Fragmentálható TechType Lista

Az alábbi lista tartalmazza a játékban szkennelhető fragmenteket és azok alapértelmezett értékeit.

### Járművek és Segédeszközök

| TechType | Magyar név | Alapértelmezett fragment szám |
|----------|------------|-------------------------------|
| `Seaglide` | Seaglide | 2 |
| `Seamoth` | Seamoth | 3 |
| `Cyclops` | Cyclops (hull/bridge/engine) | 3+3+3 |
| `Exosuit` | Exoruha (Prawn Suit) | 4 |
| `Constructor` | Mobil jármű készítő | 3 |

### Eszközök

| TechType | Magyar név | Alapértelmezett fragment szám |
|----------|------------|-------------------------------|
| `Beacon` | Jelzőbója | 2 |
| `Gravsphere` | Gravitációs gömb | 2 |
| `LaserCutter` | Lézervágó | 3 |
| `StasisRifle` | Stázispuska | 2 |
| `PropulsionCannon` | Hajtóágyú | 2 |

### Bázis modulok

| TechType | Magyar név | Alapértelmezett fragment szám |
|----------|------------|-------------------------------|
| `BaseMapRoom` | Térképszoba | 2 |
| `BaseMoonpool` | Holdfény kikötő | 2 |
| `BaseNuclearReactor` | Atomreaktor | 3 |
| `ThermalPlant` | Hőerőmű | 2 |
| `Workbench` | Módosító állomás | 3 |
| `BaseWaterPark` | Vízipark (Alien Containment) | 2 |
| `BaseBioReactor` | Bioreaktor | 2 |
| `PowerTransmitter` | Áramátviteli | 2 |

### Prawn Suit bővítmények

| TechType | Magyar név | Alapértelmezett fragment szám |
|----------|------------|-------------------------------|
| `ExosuitDrillArmFragment` | Fúrókar | 2 |
| `ExosuitGrapplingArmFragment` | Kampóskar | 2 |
| `ExosuitPropulsionArmFragment` | Hajtókar | 2 |
| `ExosuitTorpedoArmFragment` | Torpedókar | 2 |

### Seamoth bővítmények

| TechType | Magyar név | Alapértelmezett fragment szám |
|----------|------------|-------------------------------|
| `SeamothSolarCharge` | Napelemes töltő | 2 |

### Cyclops bővítmények

| TechType | Magyar név | Alapértelmezett fragment szám |
|----------|------------|-------------------------------|
| `CyclopsShieldModule` | Pajzsgenerátor | 3 |
| `CyclopsThermalReactorModule` | Termikus reaktor | 3 |

## Megjegyzések

- A szerver újraindítása szükséges a `fragments.json` módosítások érvényesítéséhez
- Ha egy TechType nem létezik, a szerver figyelmeztető üzenetet ír a logba
- A kliensek automatikusan szinkronizálódnak csatlakozáskor
- A módosítások csak a Nitrox multiplayer session-re vonatkoznak, a single player nem érintett
- Az alapértelmezett fragment számok a vanilla játékból származnak
- A Custom Fragments és Custom Recipes funkciók együtt is használhatók a maximális nehezítéshez
