import cv2
import mediapipe as mp
from processor import Processor

mp_hands = mp.solutions.hands
hands = mp.solutions.hands.Hands(model_complexity=1)
mp_drawing = mp.solutions.drawing_utils

cap = cv2.VideoCapture(0)
height, width = (int(cap.get(3)), int(cap.get(4)))
print(height, width)
# out = cv2.VideoWriter('output.mp4', cv2.VideoWriter_fourcc(*'mp4v'), 30.0, (height, width))

processor = Processor()

while True:
    ret, frame = cap.read()
    if not ret:
        break

    frame = cv2.resize(frame, (height, width))
    rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

    results = hands.process(rgb_frame)

    processor.process_results(results)
    
    if processor.left_hand_exists and processor.left is not None:
        mp_drawing.draw_landmarks(frame, processor.left, mp_hands.HAND_CONNECTIONS)

    if processor.right_hand_exists and processor.right is not None:
        mp_drawing.draw_landmarks(frame, processor.right, mp_hands.HAND_CONNECTIONS)

    cv2.imshow('Hand Tracking', frame)
    # out.write(frame)

    if cv2.waitKey(10) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()