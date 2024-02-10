from enum import Enum
import socket
import struct
from typing import Type
import json
import torch
import torch.nn as nn
import os
import numpy as np
from itertools import combinations
import urllib
import joblib

class HandLandmarks(Enum):
    WRIST = 0
    THUMB_CMC = 1
    THUMB_MCP = 2
    THUMB_IP = 3
    THUMB_TIP = 4
    INDEX_FINGER_MCP = 5
    INDEX_FINGER_PIP = 6
    INDEX_FINGER_DIP = 7
    INDEX_FINGER_TIP = 8
    MIDDLE_FINGER_MCP = 9
    MIDDLE_FINGER_PIP = 10
    MIDDLE_FINGER_DIP = 11
    MIDDLE_FINGER_TIP = 12
    RING_FINGER_MCP = 13
    RING_FINGER_PIP = 14
    RING_FINGER_DIP = 15
    RING_FINGER_TIP = 16
    PINKY_MCP = 17
    PINKY_PIP = 18
    PINKY_DIP = 19
    PINKY_TIP = 20

class Streamer:

    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    target_host = "localhost"
    target_port = 5678
    receiver_addr = (target_host, target_port)

    def __init__(self) -> None:
        pass

    def stream(self, data :bytes):
        Streamer.sock.sendto(data, Streamer.receiver_addr)

# classes = ['ThumbsUp', 'ThumbsDown', 'Pinch', 'Shoot', 'ShootClick', 'Standing', 'Peace', 'Threese', 'Yo']
classes = ['Standing', 'peace', 'ThumbsUp', 'Pinch', 'ThumbsDown', 'Threese', 'Shoot', 'ShootClick', 'Yo']


class Model(nn.Module):
    def __init__(self):
        super().__init__()
        self.layer = nn.Sequential(
            nn.Flatten(),
            nn.Linear(40 + 63, 64),
            nn.LeakyReLU(),
            nn.Linear(64, 32),
            nn.LeakyReLU(),
            nn.Linear(32, 16),
            nn.LeakyReLU(),
            nn.Linear(16, 8),
            nn.LeakyReLU(),
            nn.Linear(8, 16),
            nn.LeakyReLU(),
            nn.Linear(16, len(classes))
        )

    def forward(self, x):
        return self.layer(x)

class Message:
    landmark_mapping = {landmark.name: landmark.value for landmark in HandLandmarks}
    landmark_mapping_rev = {landmark.value: landmark.name for landmark in HandLandmarks}

    def __init__(self) -> None:
        self.buffer = None
        self.left_landmarks = None
        self.right_landmarks = None
        self.store = {
            'LeftHandExists': False,
            'Left': { key.name: {'x':0, 'y':0, 'z':0} for key in HandLandmarks },
            'RightHandExists': False,
            'Right': { key.name: {'x':0, 'y':0, 'z':0} for key in HandLandmarks }
        }
        self.min_confidence = 0.95
        self.combination = list(combinations([i for i in range(21)], 3))
        self.model = Model().to("cuda" if torch.cuda.is_available() else "cpu")
        self.download_file("https://github.com/Muthu-Palaniyappan-OL/vr_ml/releases/download/v0.4.0/model.pth", "model.pth")
        self.download_file("https://github.com/Muthu-Palaniyappan-OL/vr_ml/releases/download/v0.4.0/pca.joblib", "pca.joblib")
        self.model.layer.load_state_dict(torch.load("./model.pth"))
        self.pipeline = joblib.load("pca.joblib")
    
    def predict(self, results):
        coords = self.calculate_coords(results)
        angles = self.calculate_angles(coords)
        reduced = np.expand_dims(np.concatenate([self.pipeline.transform(np.expand_dims(angles, axis=0))[0], coords.reshape(-1)]), axis=0)
        res = self.model(torch.tensor(reduced, dtype=torch.float32))
        return res

    def calculate_coords(self, landmarks):
        """Processes and results only landmarks"""
        return np.array([[landmark.x,landmark.y,landmark.z] for landmark in landmarks])

    def calculate_angles(self, coords):
        
        def numpy_combinations(x, r):
            n = len(x)
            indices = np.arange(n)
            combinations = np.array(np.meshgrid(*[indices] * r)).T.reshape(-1, r)
            combinations = combinations[combinations[:, 0] < combinations[:, 1]]
            combinations = combinations[combinations[:, 1] < combinations[:, 2]]
            return x[combinations]

        coords = numpy_combinations(coords, 3)
        v1 = coords[:, 0] - coords[:, 1]
        v2 = coords[:, 2] - coords[:, 1]
        return np.rad2deg(np.arccos(np.sum(v1 * v2, axis=1) / np.sqrt(np.sum(v1 * v1, axis=1) * np.sum(v2 * v2, axis=1))))
    
    def builder(self) -> Type['Message']:
        self.__clear__()
        return self
        
    def left_hand(self, landmarks, existing) -> Type['Message']:
        self.store['LeftHandExists'] = True
        for mark in Message.landmark_mapping_rev:
            self.store['Left'][Message.landmark_mapping_rev[mark]]['x'] = landmarks[mark].x
            self.store['Left'][Message.landmark_mapping_rev[mark]]['y'] = landmarks[mark].y
            self.store['Left'][Message.landmark_mapping_rev[mark]]['z'] = landmarks[mark].z
        class_id, confidence = self.process_hand(landmarks)
        for id, c in enumerate(classes):
            if existing and id == class_id and confidence > self.min_confidence:
                self.store['Left'+c] = True
                self.store['Nothing'] = False
            else:
                self.store['Left'+c] = False
        return self

    def right_hand(self, landmarks, existing) -> Type['Message']:
        self.store['RightHandExists'] = True
        for mark in Message.landmark_mapping_rev:
            self.store['Right'][Message.landmark_mapping_rev[mark]]['x'] = landmarks[mark].x
            self.store['Right'][Message.landmark_mapping_rev[mark]]['y'] = landmarks[mark].y
            self.store['Right'][Message.landmark_mapping_rev[mark]]['z'] = landmarks[mark].z
        class_id, confidence = self.process_hand(landmarks)
        for id, c in enumerate(classes):
            if existing and id == class_id and confidence > self.min_confidence:
                self.store['Right'+c] = True
                self.store['Nothing'] = False
            else:
                self.store['Right'+c] = False
        return self
    
    def process_hand(self, landmark):
        output = nn.functional.softmax(self.predict(landmark), dim=1)
        print(classes[output[0].argmax()])
        return output[0].argmax(), output[0][output[0].argmax()]

    def build(self) -> bytes:
        return json.dumps(self.store).encode()
    
    def __clear__(self):
        self.store['LeftHandExists'] = False
        self.store['RightHandExists'] = False
        for pair in self.store['Left']:
            self.store['Left'][pair]['x'] = 0
            self.store['Left'][pair]['y'] = 0
            self.store['Left'][pair]['z'] = 0
        for pair in self.store['Right']:
            self.store['Right'][pair]['x'] = 0
            self.store['Right'][pair]['y'] = 0
            self.store['Right'][pair]['z'] = 0
        for c in classes:
            self.store['Right'+c] = False
        for c in classes:
            self.store['Left'+c] = False
        self.store['Nothing'] = True
    
    def download_file(self, url, destination):
        # Check if the file already exists
        if os.path.exists(destination):
            print(f"The file '{destination}' already exists.")
        else:
            # If the file doesn't exist, download it
            # print(f"Downloading file from {url} to {destination}")
            # urllib.request.urlretrieve(url, destination)
            # print(f"Download complete.")
            print(f"========================================\nPlease download this pretrained model here\n{url}\n========================================")
            raise "Pretrained Model Not Available"
