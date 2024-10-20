![RichString](Docs/Images/RichStringBanner.png)
# RichString
 A dynamic, runtime string interpolation tool for Unity, supporting custom formatting and expressions.
 ## Features:
- **Dynamic string interpolation** at runtime.
- Support for **custom formats** via `IRichStringCustomFormat`.
- **Rich text** parsing and rendering.
- **Property referencing** for objects and collections (`IEnumerable`).
- Easily extensible and configurable error handling modes.

# Get Started
 Welcome to RichString, a powerful runtime string interpolation solution designed specifically for Unity. This guide will walk you through the installation process and help you get started using RichString in your project.
## Installation
### Manual (Unity Package)
 Download the latest release from the [GitHub releases page](https://github.com/AAuraDev/RichString/releases). Simply extract the files and place the RichString folder anywhere in your Unity project’s `Assets` directory. Done!
 
 ---
 
# Basic Usage
Here’s how to quickly get started using RichString to interpolate strings at runtime.
## Example
### Simple Weapon Description
Let's walk through a simple example where we have a `Weapon` class and a `Inventory` class. We'll use RichString to dynamically reference properties from the `Weapon` and `Inventory` classes in our string expressions in runtime.

#### Define the `Weapon` Class
  Here’s a basic `Weapon` class with properties such as `Damage` and `FireRate`:
```csharp
public class Weapon
{
    [field: SerializeField] public int damage { get; private set; }
    [field: SerializeField] public int fireRate { get; private set; }
}
```
#### Define the `WeaponDescription` Class
 Here's a basic `Inventory` class with `RichString` and `MaxSlots` fields, a `Weapon` instance and a list of `Weapon`.
 `RichString` must be Initialized with the `object` that its properties is going to be referenced (in this case `this`). Initialization must be done in the `Start()` or `Awake()` Method (in a `MonoBehavior`) or anywhere **before invoking `GetParsedString()`**. 
```csharp
public class Inventory : MonoBehavior
{
    [SerializeField] RichString _description;

    [field: SerializeField] public int MaxSlots { get; set; }
    [field: SerializeField] public List<Weapon> weapons { get; private set; } = new();
    [field: SerializeField, RichReference] public Weapon mainWeapon { get; private set; }

    public void Start()
    {
      _description.Initialize(this);
    }

    public string GetDescription()
    {
      return _description.GetParsedString();
    }
}
```
#### Write Description
 texts and expressions can be written using `RichString` by following the rules in the next section [Expressions](#expressions).

# Expressions
 **Rules for Writing Expressions with RichString**

 This section outlines the rules for crafting expressions using the powerful RichString system, which allows for seamless property referencing and rich text formatting. By following these guidelines, you can create dynamic strings that enhance the   interactivity and visual appeal of your content.

 While the specifiers shown here are the defaults, they can be customized to fit your needs via `RichStringSettings` in the asset folder.
## **Property Referencing**
 
 - **Property Reference**
   
   To reference a property, simply place the property name within curly braces {}:
   
   ```
   Your Inventory has {MaxSlots} max slots.  
   ```

 - **Property in an Instance**

   To reference a property of a nested object, use a dot `.` to access the property, just like you would in regular code:

   ```
   Your Main Weapon deals {mainWeapon.damage} damage.
   ```

 - **Element of an `IEnumerable`**

   To access an element from an IEnumerable, use the arrow `->` to separate the property name from the desired index. You can also nest these references to access properties of objects contained within collections:

   ```
   Your First Weapon deals {weapons->0.damage}
   ```
