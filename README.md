## WIP
Works as a proof of concept, needs refactoring, testing, all the following...
-clearer separation/purpose of static vs serialized attributes
-virtual functions and classes that allow overriding functionality
-generic equipment slots with variable rules
-inspector overrides for debugging and viewing inventory state
-item database meta data modifier, things like assigning groups, rarities, other meta information to items
-base UI class, example UI class, extendable UI and instructions
-example usage on character/in game 
-clearer state control, probably an event class with easier/more obvious callback methods
-clean up/split up inventory state to have clearer purpose
-view serialized data from editor
-saving/loading with slot/id designations
-different save types editable/encrypted
-testing suite for saving/loading inventory state, changing equipment, adjusting stack sizes, other state changes
-consolidate directories into an Inventory directory with intuitive navigation
-clean up editor scripts make intuitive to modify/extend for added functionality
-fault tolerance, reset or salvage if corrupted save data or item database
-much more

# MasterInventory
MasterInventory is a standalone generic inventory system for Unity.

It enables the use of a generic inventory system that is based on key value pairs to allow total separation of game logic and inventory logic without the need for any additional helper classes.

It is implemented by making use of a generic ScriptableObject based item database and making use of generic and hard coded item conditions and value types. 

## System Requirements

Unity 2017 or later. May work on older versions.

## Inventory Types
These are the different data and inventory types that allow the database to function.

### InventoryItem
This is the base and only class for items to be created from. The inventory combines all existing InventoryItems into a database that is managed based on ItemType data.
The InventoryItem also contains all information needed to be utilized in game. Data is stored as a list of serialized and non serialized key value pairs in the form of "ItemAttributes."

### ItemType
ItemType acts as a meta data holder and is assigned to InventoryItems to tell the inventory how to manage it. You can extend it to create any type of item and then define its use.
There are included types such as ResourceType, EquippabeType, and ConsumableType. This is where you assign values that will be used among all like-typed items. 
This can include information that the InventoryState will use to manage it as well as info that all like-items will utilize in game such as a ConsumableTypes use time, or a ResourceTypes stack size.

### EquipmentSlot
The EquipmentSlot is used to store any item in an "equipped" state as opposed to existing in the main inventory pool. It doesn't take up inventory space and has callbacks for equipped state changes.
The EquipmentSlot has assignable valid equipment types, and the default equipment slot can be set by the inventory database, this allows multiple slots for the same ItemType such as having multiple rings.

### InventorySlots
InventorySlots is the general holding area for InventoryItems not existing in some other form such as equipment slots. It keeps track of the inventory state by pairing slots to item instances and enables callback functions for use by the UI or game logic.

## Inventory and InventoryState
The inventory state coupled with the inventory database are the core components of the inventory.

### Inventory 
The inventory acts as the middleman between all systems, it creates the item database, initializes the Inventory State, and provides functions for utilizing the InventoryState

### InventoryState
The inventory state is a class that holds and manages the data for the inventory, every other system gets it's information from the state, no state data should be held or considered safe from any other source.
The state holds all state data that allows the inventory to be saved and loaded.
It manages the inventory by holding a list of "ItemData" which each represent an item that the inventory must manage and update. The ItemData holds all information needed to tell any system what that item is doing, as well as any data needed to recreate the inventory state on load.

## How to create your own inventory
### Create an ItemDatabase 
The Inventory class will auto construct the database from items existing in the project assets. Because all information about the item that the inventory cares about comes from the ItemType the only thing required to get the database to recognize your InventoryItem is an item type.
After creating 1 or more InventoryItems with right click -> Create -> Inventory -> InventoryItem, you will need an ItemType to assign to it.
Create an ItemType the same way, ItemTypes can not be generic and you need to use one of the existing types or create your own. Creating a weapon type will show some additional functionality.
The WeaponType will have information such as it's default slot, or whether it's stackable.
After you assign an ItemType to an InventoryItem, the database will populate it into the database. The inventory and inventory state will already know how to handle it if it is a preexisting type, if it is a new type it will need functionality added on how it is handled in cases such as when it is used, when it is equipped, when it is picked up, when it is unequipped.

### Use in your game logic

### Using an Inventory UI
