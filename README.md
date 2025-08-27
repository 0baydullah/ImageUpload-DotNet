### Dependency for image resizing

- SixLabors.ImageSharp
- version 3.1.11
- https://www.nuget.org/packages/SixLabors.ImageSharp/3.1.11
- Install-Package SixLabors.ImageSharp -Version 3.1.11
- dotnet add package SixLabors.ImageSharp --version 3.1.11
- Repo: https://github.com/SixLabors/ImageSharp
- Published Date : Wednesday, July 30, 2025 (7/30/2025)
- License: Apache-2.0


```cs
// usings
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

// file creation and resize
using (var image = await Image.LoadAsync(userDto.image.OpenReadStream()))
{
    image.Mutate(x => x.Resize(Helper.DpWidth, Helper.DpHeight));
    var encoder = new JpegEncoder { Quality = 75 };
    await image.SaveAsync(filePath, encoder);
}
```


```cs
 // file creation
 using (var stream = new FileStream(filePath, FileMode.Create))
 {
     await userDto.image.CopyToAsync(stream);
 }
 ```