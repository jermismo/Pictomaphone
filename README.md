# Pictomaphone
The best photo editing app for Windows Phone 7.

## Introduction
I've decided to make my old WP7 app open source, so others can learn from how I implemented photo editing.

Windows Phone 7 had some tough limitations that made creating a performant photo editor difficult. 

- The image size for applications was capped at 3 megapixels, even though the phones took higher resolution images.
- Apps ran on Silverlight, which was not very optimized for ARM chips and had its own limitations
- There wasn't a way to call ARM intrinsics for vectorization of math operations

