# PropolisEngine

Propolis Game Engine in Unity

![alt text](https://github.com/ProjetPropolis/PropolisEngine/blob/master/UI.png?raw=true)

## UI Usage

  The Control Panel in the engine is divided in 3 sections:

* ### Console :
    In the console mode, you can see all commands behing send to the PropolisEngine and send some manual commands. 
    (See documentations above on how to format commands)

* ### Animations
    All animations used in game represented as buttons to trigger them manualy.

* ### Configure
    All tools needed to modify and configure the visual representation of the Ruche and the Champs Moléculaires.
    The 3 main tools are :

  #### Delete
    Simply click on delete then click on the group you want to delete.
  #### Create
    Click on delete then click where you want to instanciate a group. A popup window will apear
    where you can enter the follwing info : ID, Ip , PortIn and Port Out.
  #### Setup
    Simply click on edit then click on the group you want to modify. A popup window will apear
    where you can enter the follwing info : ID, Ip , PortIn and Port Out.
    At this pont you can use you keyboard arrows to move the group around.

  Following all modification please use the save button on the bottom of the Setup Panel.
  
## Keybaord Shortcuts in UI

Esc - Open/Close Control Panel

Tab - Switch from one tab to the other in Control Panel

R - Switch to Ruche View

M - Switch to Champ Moléculaires View

A - Switch to All View

## Propolis Export

While the game is running Propolis Export send the following OSC commands

### Hex Status ###

     /hex [*HEX_ID*] [*STATUS*]
  
### Atom Status ###

    /atom [*ATOM_ID*] [*STATUS*]
  
### Battery Status ###

*Please note that the status will be a float value between 0.0 and 1.0*

    /battery [*STATUS*]
  

## Propolis Engine Console Script

The propolis engine comes with a console script. All the actions availlable in the engine can be done.

Each part of a command must be seperated by a space. Otherwise the parser will return an error.

### Item Status ###

OFF = 0

ON =  1

CORRUPTED = 2

ULTRACORRUPTED = 3

CLEANSER = 4

RECIPE1 = 5

RECIPE2 = 6

RECIPE3 = 7


### Command List


#### CREATE
  
  Create a new group element
  
    CREATE [type] [params ....]
  
  ##### HexGroup
  
    CREATE HEXGROUP [*GroupId*] [*X*] [*Y*] [*IP_ADDRESS*] [*IN_PORT*] [*OUT_PORT*]
  
  ##### AtomGroup
  
    CREATE ATOMGROUP [*GroupId*] [*X*] [*Y*] [*IP_ADDRESS*] [*IN_PORT*] [*OUT_PORT*]
  
#### DELETE
  
  Delete a group element
    
    DELETE [*type*] [*GroupID*]
  
  ##### HexGroup
  
    DELETE HEXGROUP [*GroupID*]
  
  ##### AtomGroup
  
    DELETE ATOMGROUP [*GroupID*]
  
#### UIS
  
  Update the status of a single item or all item of a group
  
  ##### Single Item
  
    UIS [*type*] [*GroupID*] [*ItemID*] [*Status*]
  
  ##### All Items
  
    UIS [*type*] [*GroupID*] ALL [*Status*]
  
  ##### AtomGroup
  
    UIS ATOMGROUP [*GroupID*] [*ItemID*] [*Status*] 
  
    UIS ATOMGROUP [*GroupID*] ALL [*Status*] 
  
  ##### HexGroup
  
    UIS ATOMGROUP [*GroupID*] [*ItemID*] [*Status*] 
  
    UIS ATOMGROUP [*GroupID*] ALL [*Status*]
  
#### PLAY
Start the gameplay
 
    PLAY
  
#### STOP
Stop the gameplay

    STOP


#### LOAD
Load the last saved engine state

    LOAD


#### SAVE
Save the last saved engine state

    SAVE


#### UBL
Update the battery level. Only values from 0.0 to 1.0 are accepted as level values

    UBL [*BatteryLevel*]


  
