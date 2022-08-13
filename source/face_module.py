from mtcnn import MTCNN
from cv2 import cv2
from PIL import Image

import numpy as np


detector = MTCNN()

def face_detect(img):
	mt_face_full = detector.detect_faces(img)

	face_rect_ls = []
	face_center_ls = []

	for idx, face in enumerate(mt_face_full):
		
		mt_face = face
		print(face)

		x, y, w, h = mt_face['box']
		face_rect_ls.append(mt_face['box'])
		
		center = [x+(w/2), y+(h/2)]
		max_border = max(w, h)
		
		# center alignment
		left = max(int(center[0]-(max_border/2)), 0)
		right = max(int(center[0]+(max_border/2)), 0)
		top = max(int(center[1]-(max_border/2)), 0)
		bottom = max(int(center[1]+(max_border/2)), 0)
		
		# crop the face
		center_img_k = img[top:top+max_border, 
						   left:left+max_border, :]

		center_img = Image.fromarray(center_img_k).resize([160, 160])
		center_img.save("./saved_image/{}.png".format(idx))
		center_img = np.array(center_img)

		face_center_ls.append(center_img)


	return face_rect_ls, face_center_ls

