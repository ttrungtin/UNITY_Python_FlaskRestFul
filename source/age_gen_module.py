from tensorflow.keras.models import load_model

model_age = load_model('./model/age_model_facenet.h5')
model_gender = load_model("./model/gender_model.h5")

def age_gender_detect(img):

	img = img / 255

	age_pred = model_age.predict(img.reshape(1, 160, 160, 3))
	gender_pred = model_gender.predict(img.reshape(1, 160, 160, 3))


	age = int(age_pred[0][0])
	gender = 0 if gender_pred[0][0] <= 0.5 else 1

	print(age_pred, gender_pred)

	return age, gender