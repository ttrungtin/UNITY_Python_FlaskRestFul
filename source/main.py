from flask import Flask
from flask_restful import Api, Resource
from cv2 import cv2

from face_module import face_detect
from age_gen_module import age_gender_detect

app = Flask(__name__)
api = Api(app)



img_dir = r'./saved_image/recorded.png'

button = {
	"Space": {"button": "Pressed Space"},
	"Enter": {"button": "Presed Enter"},
	"Submit": {"button": "Pressed Submit"}
}

class Hello(Resource):
	def post(self):
		return {"age": "1"}
	def get(self, button_id):
		return button[button_id]


'''
PythonFaceList
- PythonFaceStructure[]

PythonFaceStructure
- string age
- string gender
- int[] faceRectangle
'''
class TestServer(Resource):
	def get(self):
		print("Test server called")
		return {"faceList": [25, 100, 1, 2, 3 , 4, 25, 200, 1, 2, 3, 4]}
		# return {"faceList": [25, 100, 1, 2, 3 , 4]}



class Trigger(Resource):
	def get(self):

		result = []

		# load image
		print("Load Image...")
		img = cv2.imread(img_dir)
		print(img.shape)

		# face detect
		print("Face Detect...")
		face_rect_ls, face_center_ls = face_detect(img)

		for idx, face_center in enumerate(face_center_ls):
			# age/gender detect - return as int
			print("Age/Gen Detect {}...".format(idx))
			age, gender = age_gender_detect(face_center)
			result += [age, gender] + face_rect_ls[idx]

		print(result)
		return {"faceList": result}		


api.add_resource(Hello, "/hello/<string:button_id>")
api.add_resource(Trigger, "/trigger/")
api.add_resource(TestServer, "/test/")


if __name__ == "__main__":
	app.run(debug=False)