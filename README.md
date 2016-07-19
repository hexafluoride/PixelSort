# PixelSort
Sorts a given picture's pixels for a cool animation.

## Running PixelSort

    pixelsorter /path/to/image
	
For example, if the target path was `/home/hexafluoride/test.png`, the output images would be located in `/home/hexafluoride/test/test-%d.png` where `%d` is frame number.

## Compiling PixelSort

To compile PixelSort, run

	msbuild

or
	
	xbuild
	
at the project root.

## License

PixelSort is distributed under the MIT License.