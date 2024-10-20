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
Let's walk through a simple example where we have a `Weapon` class and a `WeaponDescription` class. We'll use RichString to dynamically reference properties from the Weapon class in our string descriptions in runtime.

#### Define the `Weapon` Class
  Here’s a basic `Weapon` class with properties such as `Damage` and `FireRate`:
```csharp
public class Weapon
{
    [field: SerializeField] public int Damage { get; set; }
    [field: SerializeField] public int FireRate { get; set; }
}
```
#### Define the `WeaponDescription` Class and reference `Weapon`
  Initialize the RichString with the `object` you want to reference its properties in the `Start()` Method or anywhere **before invoking `GetParsedStrig()`**. 
```csharp
public class WeaponDescription : MonoBehavior
{
    [SerializeField] RichString _description;
    [SerializeField] Weapon _mainWeapon;

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

