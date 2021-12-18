# TerraRoyale
TShock plugin that automatically teams and spaces players out.

## About
- The CMD **/battlestart** is used to initiate the battle.
- The CPU determines the required spacing needed for teams and generated platforms.
- Un-teamed players are added to a random team.
- Players are teleported to their respected team-colored platforms.

This is how the CPU determins where to place the teams. Keep in mind the goal is to place players as far away from each other as possible. You are allowed to have two players in one team. The teams will **ALWAYS** follow this order.

![Untitled](https://user-images.githubusercontent.com/33048298/146626369-3d07c1aa-fb27-4105-a0e8-dd550d5c1a3e.png)

**PLACEMENT CHART:**
- 1 Unique Teams - The game will not start.
- 2 Unique Teams - Team one starts left ocean, team two stars right ocean.
- 3 Unique Teams - Team one starts left ocean, team two stars right ocean, team three starts left 50%.
- 4 Unique Teams - Team one starts left ocean, team two stars right ocean, team three starts left 50%, team four starts right 50%.
- 5 Unique Teams - Team one starts left ocean, team two stars right ocean, team three starts left 50%, team four starts right 50%, team five starts in middle.

## Permissions
``battlestart.use`` - Allows players to use /battlestart
