# Unity3d_Toolkit
Powerful tools to cover some use-cases in any unity3d game from me!
All parts of toolkit tested and completely working, but could be not intuitive for someone else who is not me, so you can perceive current state as "Alpha Release".

# **Requirements**
Remember: you SHOULD buy assets that is marked as "Paid" before use them.
### **Hard Requirements**
- Odin Inspector(Paid): https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041
- Odin Validator(Paid): https://odininspector.com/odin-project-validator
- DOTWeen(Free)

### **Soft Requirements**
- Animancer Lite(Free) (Panel.cs, Extensions.cs)
- Easy Save 2(Paid) (ProcessorSaveLoad.cs): https://assetstore.unity.com/?q=Easy+Save&orderBy=1

You can remove or replace code associated with those assets. Replace Animancer usages with Animator. Remove ProcessorSaveLoad.cs or replace "Easy Save 2" usages with your solutions.

# What inside?
- Complex highly customizable characteristic system for your characters, items etc.
- Simple Save/Load with migration feature(you can update save file version of client to avoid errors and progress lose)
- Sound Pool System
- ValuePipeline system
- Complex Time-management system with TimeZone's
- UI: Basic Panel.cs class for any your panels.
-- Panel.cs Basic class for any your panels.
-- SimpleGrid.cs for complex UI-grid use-cases. You can it use as an alternative for Layout Groups
-- ProgressBar*.cs simple solution for your progress bars
-- MonoScalerUI.cs for configuring UI scaling depending on screen resolution
- Complete GUID system! For any items in your game(Levels, Characters, Items, Achievements, etc-etc)
- Simple Coroutines system
- Observer system
- Extensions.cs useful extensions
