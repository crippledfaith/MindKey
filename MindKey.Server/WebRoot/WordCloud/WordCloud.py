from wordcloud import WordCloud, STOPWORDS
from PIL import Image
import numpy as np
from os import path
import random

def grey_color_func(word, font_size, position, orientation, random_state=None,**kwargs):
    return "hsl(0, 0%%, %d%%)" % random.randint(60, 100)


alice_mask = np.array(Image.open(mask_path))

wc = WordCloud(background_color="#222222", max_words=2000, mask=alice_mask,
              contour_width=1, contour_color='steelblue')
wc.generate(wordlist)
wc.recolor(color_func=grey_color_func, random_state=3)
path = output_path
wc.to_file(path)