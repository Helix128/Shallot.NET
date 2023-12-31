# Shallot.NET
![image](https://github.com/Helix128/Shallot.NET/assets/15237757/7c23d2b2-fbbd-4d24-86a1-66acbf480a99)

(Unofficial) Dragon Ball Legends Character Data API

### C# port of [DBLegendsAPI](https://github.com/feijoes/DBlegendsAPI) by [feijoes](https://github.com/feijoes)


## Dependencies
Shallot.NET includes the following packages:
- [Newtonsoft.JSON](https://github.com/JamesNK/Newtonsoft.Json/tree/master)
- [HTMLAgilityPack](https://github.com/zzzprojects/html-agility-pack/tree/master)
- [HtmlAgilityPack.CssSelectors.NetCore](https://github.com/trenoncourt/HtmlAgilityPack.CssSelectors.NetCore)

# How to use

- Add ```using Shallot;``` to the beginning of your script.

## Functions

### GetCharactersByName(string name)

Returns a ```List<Character>``` containing every character with the specified name.

For instance, ```Api.GetCharactersByName("Broly")``` returns a list containing every Broly character.

```Api.GetCharactersByName("Legendary Super Saiyan Broly")``` returns a list only containing LSSJ Broly characters.

### GetCharacterById(string id)

Returns a ```Character``` with the specified ID.

For instance, ```Api.GetCharacterById("DBL54-05U")``` returns [UL Gogeta Blue](https://legends.dbz.space/characters/508).

# TODO

- Achieve functional parity between Shallot.NET and DBLegendsAPI
- Optimize download system
- Fix Unity compatibility
  
### Possible additions
- Get summon data

# License

Shallot.NET is released under the MIT License. Feel free to do anything you want with this!

# Credits

- [feijoes](https://github.com/feijoes): Original API in Rust
- [dbz.space](https://legends.dbz.space/): Data
