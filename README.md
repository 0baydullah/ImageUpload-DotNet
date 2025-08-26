```
 // file creation
 using (var stream = new FileStream(filePath, FileMode.Create))
 {
     await userDto.image.CopyToAsync(stream);
 }
 ```
