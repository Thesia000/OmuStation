Evening this file is here to explain the reasoning behind some of the design desition and how the Multistruct that is the silo operates.

For a base line we have the Core denoted in the diagrams as [C] The core is the main interaction field, other interaction enteties exist denoted as [i]

Connectors are present they only coem in 3 forms:
- T junctions
- Stright
- Corner (north to east and north to west)
A complete junction was not added to add additional complexity to the construction of the silo, can theoretically be achived using larger silo storage units

Storage units hold materials they should generally obay the following formular:

[Materials Stored] = 100 * [OverallSize of the StorageStructure in Tiles] ^ 2 * ( 1 / [Material tier] ^ 2)

[OverallSize of the StorageStructure in Tiles] = number of tiles the structure takes

[Material tier] = the rarity tier of the material:
Tier 1:
- Steel
- Glass
- Plastic
Tier 2:
- Uranium
- Gold
- Silver
Tier 3:
- Plasma

Research should not offer better small constructs but enable construction of ever larger structures.
They are summerised into:

Tier 1 Storage:
- 1 tile limit
- unique for each material

Tier 2 Storage:
- 4 tile limit
- unique per tier of material

Tier 3 Storage
- no tile cap
- configurable for any mat

WARNING to declare new materials please look at the SignalSalvage system thanks ^^
File contains the words: THIS_FILE_TO_ADD_NEW_MATERIALS
and can be found at Content.Omu.Server/_BSD/SignalSalv/Helpers/MaterialListsMatTypes.cs