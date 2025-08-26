```
 // file creation
 using (var stream = new FileStream(filePath, FileMode.Create))
 {
     await userDto.image.CopyToAsync(stream);
 }
 ```

```
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
