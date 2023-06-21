# FishNetHelpers README

1. `HathoraPlayer` replaces FishNet's default `Player`

2. HathoraDemoPrefabObjects is used in place of DefaultPrefabObjects to replace FishNet's default `Player` with `HathoraPlayer`.

3. `NestedDefaultFishNetPrefabObjs` replaces FishNet's default `DefaultPrefabObjects`, since this is auto-generated at the root of /Assets (and the plugin is nested in a singular /Hathora dir). This allows us to serialize this in scenes to prevent errors saying the DefaultPrefabObject is missing (since we can't include the root object as a store plugin).
