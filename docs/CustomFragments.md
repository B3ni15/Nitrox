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
| `TechType` | string | A **fragment** TechType neve (pl. "SeamothFragment", "SeaglideFragment") - NEM a végeredmény! |
| `TotalFragments` | int | Hány fragment szkennelése szükséges a feloldáshoz |

**FONTOS:** A TechType-nak a FRAGMENT nevét kell használni (pl. `SeamothFragment`), NEM a feloldott technológia nevét (pl. `Seamoth`)!

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
    { "TechType": "SeaglideFragment", "TotalFragments": 5 },
    { "TechType": "SeamothFragment", "TotalFragments": 8 },
    { "TechType": "CyclopsHullFragment", "TotalFragments": 5 },
    { "TechType": "CyclopsBridgeFragment", "TotalFragments": 5 },
    { "TechType": "CyclopsEngineFragment", "TotalFragments": 5 },
    { "TechType": "ExosuitFragment", "TotalFragments": 10 },
    { "TechType": "ConstructorFragment", "TotalFragments": 6 },
    { "TechType": "BeaconFragment", "TotalFragments": 4 },
    { "TechType": "GravSphereFragment", "TotalFragments": 4 },
    { "TechType": "LaserCutterFragment", "TotalFragments": 5 },
    { "TechType": "StasisRifleFragment", "TotalFragments": 5 },
    { "TechType": "PropulsionCannonFragment", "TotalFragments": 5 },
    { "TechType": "WorkbenchFragment", "TotalFragments": 6 },
    { "TechType": "BaseNuclearReactorFragment", "TotalFragments": 6 },
    { "TechType": "ThermalPlantFragment", "TotalFragments": 5 },
    { "TechType": "BaseMapRoomFragment", "TotalFragments": 5 },
    { "TechType": "MoonpoolFragment", "TotalFragments": 6 }
  ]
}
```

### Könnyített mód (kevesebb fragment)

```json
{
  "Fragments": [
    { "TechType": "SeamothFragment", "TotalFragments": 2 },
    { "TechType": "CyclopsHullFragment", "TotalFragments": 1 },
    { "TechType": "CyclopsBridgeFragment", "TotalFragments": 1 },
    { "TechType": "CyclopsEngineFragment", "TotalFragments": 1 },
    { "TechType": "ExosuitFragment", "TotalFragments": 2 },
    { "TechType": "LaserCutterFragment", "TotalFragments": 1 },
    { "TechType": "StasisRifleFragment", "TotalFragments": 1 }
  ]
}
```

## Fragment TechType Lista

Az alábbi lista tartalmazza a játékban szkennelhető fragmentek **TechType neveit** és azok alapértelmezett értékeit.

**FONTOS:** A JSON-ban a "Fragment" végződésű neveket kell használni!

### Járművek és Segédeszközök

| Fragment TechType | Feloldott technológia | Alapértelmezett |
|-------------------|----------------------|-----------------|
| `SeaglideFragment` | Seaglide | 2 |
| `SeamothFragment` | Seamoth | 3 |
| `CyclopsHullFragment` | Cyclops (törzs) | 3 |
| `CyclopsBridgeFragment` | Cyclops (híd) | 3 |
| `CyclopsEngineFragment` | Cyclops (motor) | 3 |
| `ExosuitFragment` | Exoruha (Prawn Suit) | 4 |
| `ConstructorFragment` | Mobil jármű készítő | 3 |

### Eszközök

| Fragment TechType | Feloldott technológia | Alapértelmezett |
|-------------------|----------------------|-----------------|
| `BeaconFragment` | Jelzőbója | 2 |
| `GravSphereFragment` | Gravitációs gömb | 2 |
| `LaserCutterFragment` | Lézervágó | 3 |
| `StasisRifleFragment` | Stázispuska | 2 |
| `PropulsionCannonFragment` | Hajtóágyú | 2 |

### Bázis modulok

| Fragment TechType | Feloldott technológia | Alapértelmezett |
|-------------------|----------------------|-----------------|
| `BaseMapRoomFragment` | Térképszoba | 2 |
| `MoonpoolFragment` | Holdfény kikötő | 2 |
| `BaseNuclearReactorFragment` | Atomreaktor | 3 |
| `ThermalPlantFragment` | Hőerőmű | 2 |
| `WorkbenchFragment` | Módosító állomás | 3 |
| `BaseWaterParkFragment` | Vízipark (Alien Containment) | 2 |
| `BaseBioReactorFragment` | Bioreaktor | 2 |
| `PowerTransmitterFragment` | Áramátviteli | 2 |
| `BatteryChargerFragment` | Elemtöltő | 2 |
| `PowerCellChargerFragment` | Energiacella töltő | 2 |

### Prawn Suit bővítmények

| Fragment TechType | Feloldott technológia | Alapértelmezett |
|-------------------|----------------------|-----------------|
| `ExosuitDrillArmFragment` | Fúrókar | 2 |
| `ExosuitGrapplingArmFragment` | Kampóskar | 2 |
| `ExosuitPropulsionArmFragment` | Hajtókar | 2 |
| `ExosuitTorpedoArmFragment` | Torpedókar | 2 |

### Seamoth bővítmények

| Fragment TechType | Feloldott technológia | Alapértelmezett |
|-------------------|----------------------|-----------------|
| `SeamothSolarChargeFragment` | Napelemes töltő | 2 |

### Cyclops bővítmények

| Fragment TechType | Feloldott technológia | Alapértelmezett |
|-------------------|----------------------|-----------------|
| `CyclopsShieldFragment` | Pajzsgenerátor | 3 |
| `CyclopsThermalReactorFragment` | Termikus reaktor | 3 |

## Megjegyzések

- A szerver újraindítása szükséges a `fragments.json` módosítások érvényesítéséhez
- Ha egy TechType nem létezik, a szerver figyelmeztető üzenetet ír a logba
- A kliensek automatikusan szinkronizálódnak csatlakozáskor
- A módosítások csak a Nitrox multiplayer session-re vonatkoznak, a single player nem érintett
- Az alapértelmezett fragment számok a vanilla játékból származnak
- A Custom Fragments és Custom Recipes funkciók együtt is használhatók a maximális nehezítéshez
