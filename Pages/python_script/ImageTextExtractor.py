import sys
from PIL import Image
import pytesseract

def image_to_text(image_path):
    img = Image.open(image_path)
    custom_config = r'--oem 3 --psm 6'
    text = pytesseract.image_to_string(img, lang='eng', config=custom_config)
    return text

input_string = sys.argv[1]
result = image_to_text(input_string)
print(result)