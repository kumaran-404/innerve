from enum import Enum
from streamer import Message, Streamer

class Processor:
    THRESHOLD = 0.4

    def __init__(self, history_size = 5) -> None:
        self.history_size = history_size
        self.left_hand_exists = False
        self.right_hand_exists = False
        self.left = None
        self.right = None
        self.message = Message()
        self.streamer = Streamer()

    def process_results(self, results :any):
        multi_hand_landmarks = results.multi_hand_landmarks                 # for hand landmarks drawing in Video
        multi_hand_world_landmarks = results.multi_hand_world_landmarks     # for unity
        multi_handedness = results.multi_handedness                         # is it left or right
        
        self.left_hand_exists = False
        left_hand_index = -1
        self.right_hand_exists = False
        right_hand_index = -1

        if multi_hand_world_landmarks:

            for i in range(len(multi_hand_world_landmarks)):
                if self.left_hand_exists and self.right_hand_exists: break

                if not self.left_hand_exists and multi_handedness[i].classification[0].label == "Right":
                    self.left_hand_exists = True
                    left_hand_index = i

                if not self.right_hand_exists and multi_handedness[i].classification[0].label == "Left":
                    self.right_hand_exists = True
                    right_hand_index = i


        # compare old and new value and update right hand
        if self.left_hand_exists and self.left is not None:
            # if NumericalProcessor.calculate_cost(self.left.landmark, multi_hand_landmarks[left_hand_index].landmark) > Processor.THRESHOLD:
            self.left = multi_hand_landmarks[left_hand_index]

        # compare old and new value and update left hand
        if self.right_hand_exists and self.right is not None:
            # if NumericalProcessor.calculate_cost(self.right.landmark, multi_hand_landmarks[right_hand_index].landmark) > Processor.THRESHOLD:
            self.right = multi_hand_landmarks[right_hand_index]

        # New Hand, No Old value to update
        if self.left_hand_exists and self.left is None:
            self.left = multi_hand_landmarks[left_hand_index]

        # New Hand, No Old value to update
        if self.right_hand_exists and self.right is None:
            self.right = multi_hand_landmarks[right_hand_index]
            # print(multi_hand_world_landmarks[self.right_hand_index].landmark[0].x)

        self.message = self.message.builder()

        if self.left_hand_exists:
            self.message.left_hand(self.left.landmark, self.left_hand_exists)

        if self.right_hand_exists:
            self.message.right_hand(self.right.landmark, self.right_hand_exists)
        
        val = self.message.build()
        # print(val)
        # print()
        c = 'ThumbsDown'
        # print(self.message.store['Left' + c] if f'Left{c}' in self.message.store else None, self.message.store['Right' + c] if f'Right{c}' in self.message.store else None)
        self.streamer.stream(val)


class NumericalProcessor:

    @staticmethod
    def calculate_cost(landmarks_old, landmarks_new):
        cost = 0

        for i in range(len(landmarks_old)):
            cost += abs(landmarks_old[i].x - landmarks_new[i].x)
            cost += abs(landmarks_old[i].y - landmarks_new[i].y)
            cost += abs(landmarks_old[i].z - landmarks_new[i].z)
        
        return cost
    

