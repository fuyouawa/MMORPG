////////////////////////////////////////////////////////////////////////////////////////////
                       Hyper Casual FX Pack (by. Kyeoms)
////////////////////////////////////////////////////////////////////////////////////////////

Thank you for purchasing the Hyper Casual FX Pack.
This note describes how this package is configured, how texture should be used, and how it works within a Particle System.

This effect is designed to work in a Built-in, URP or HRRP.
To use in Built-in, you have to install "Shader Graph" from package manager.

I put all the effect elements into one textures so that each prefab uses the least amount of material.

The structure of the texture is as follows.

   ▷ Red channel is main texture.
   ▷ Green channel is dissolve texture. The main texture gradually dissolve into the shape of green texture.
   ▷ Blue channel is for secondary color.
   ▷ And Alpha channel.

These effects can be modified by two Custom Data in the Particle System.

There are 4 Components in Custom Data 1.

   ▷ X value is for Dissolve. From 0 to 1, it gradually dissolves.
   ▷ Y value is for Dissolve Sharpness. The larger the number, the sharper the edges of dissolve.
   ▷ Z value is for Emissive Power. The larger the number, the stronger emission.
   ▷ W value is for Soft Particle Factor. The larger the number, the more transparent the mesh and overlapping particles become.

You can use Custom Data 2 to add Secondary colors.
If you don't want to use the Secondary colors, change the custom data 2 mode to 'Disabled'.

Material and shader named "VFX_lab" are not used for effects. It was used in the background of Scene just to show the effect.

If your project environment is 2D or 2D Experimental and you can't see the effect, please set the "Use SoftParticle Factor?" bool parameter of all materials to off.

Thank you once again, and I hope my effect will be useful for your development.
- Kyeoms