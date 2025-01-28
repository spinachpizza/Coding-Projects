import torch
import torch.nn as nn
import torch.nn.functional as F
from torch.utils.data import TensorDataset, DataLoader
from torchvision import datasets
from torchvision.transforms import transforms
from torch.optim.lr_scheduler import StepLR

import numpy as np

class ResidualBlock(nn.Module):

    def __init__(self, inchannels, outchannels, stride=1, downsample=None):
        super(ResidualBlock, self).__init__()

        #First convolution block
        self.conv1 = nn.Sequential(
            nn.Conv2d(inchannels, outchannels, kernel_size=3, stride=stride, padding=1),
            nn.BatchNorm2d(outchannels),
            nn.ReLU()
        )

        #Second convolution block
        self.conv2 = nn.Sequential(
            nn.Conv2d(outchannels,outchannels, kernel_size=3, stride=1, padding=1),
            nn.BatchNorm2d(outchannels)
        )

        self.downsample = downsample
        self.relu = nn.ReLU()
        self.outchannels = outchannels

    def forward(self, x):
        residual = x #Store original for skip connection
        x = self.conv1(x)
        x = self.conv2(x)
        #If downsample is required adjust
        if self.downsample:
            residual = self.downsample(residual)
        x += residual
        x = self.relu(x)
        return x


class ResNet(nn.Module):
    def __init__(self, block, layers):
        super(ResNet, self).__init__()
        self.inplanes = 64
        self.conv1 = nn.Sequential(
            nn.Conv2d(3, 64, kernel_size=3, stride=1, padding=1),
            nn.BatchNorm2d(64),
            nn.ReLU()
        )

        self.maxpool = nn.MaxPool2d(kernel_size=3, stride=2, padding=1)

        #Layer definitions
        self.layer1 = self.make_layer(block, 64, layers[0], stride=1)
        self.layer2 = self.make_layer(block, 128, layers[1], stride=2)
        self.layer3 = self.make_layer(block, 256, layers[2], stride=2)
        self.layer4 = self.make_layer(block, 512, layers[3], stride=2)

        self.pool = nn.AdaptiveAvgPool2d((1,1))
        self.fc = nn.Linear(512, 100)


    def make_layer(self, block, planes, blocks, stride):
        downsample = None
        if stride != 1 or self.inplanes != planes:

            downsample = nn.Sequential(
                nn.Conv2d(self.inplanes, planes, kernel_size=1, stride=stride),
                nn.BatchNorm2d(planes)
            )
        layers = []
        layers.append(block(self.inplanes, planes, stride, downsample))
        self.inplanes = planes
        for i in range(1, blocks):
            layers.append(block(self.inplanes, planes))

        return nn.Sequential(*layers)


    def forward(self, x):
        x = self.conv1(x)
        x = self.layer1(x)
        x = self.layer2(x)
        x = self.layer3(x)
        x = self.layer4(x)
        x = self.pool(x)
        x = torch.flatten(x,1)
        x = self.fc(x)
        return x





#Apply transformations for data augmentation
transform = transforms.Compose([
    transforms.RandomHorizontalFlip(),
    transforms.RandomCrop(32, padding=4),
    transforms.RandomRotation(10),
    #transforms.ColorJitter(brightness=0.2, contrast=0.2, saturation=0.2, hue=0.1),
    #transforms.GaussianBlur(kernel_size=3),
    transforms.ToTensor(),
    transforms.Normalize((0.5071,0.4867,0.4408),(0.2675,0.2565,0.2761))
])

model = ResNet(ResidualBlock, [3,4,6,3])
cuda = torch.cuda.is_available()
if(cuda):
  model.cuda()


#Load data from CIFAR100 dataset
test_data = datasets.CIFAR100(root='./data', train=False, download=True,
                                         transform=transform)
train_data = datasets.CIFAR100(root='./data', train=True, download=True,
                                         transform=transform)

train_loader = DataLoader(train_data, batch_size=64, shuffle=True,
                          num_workers=2)
test_loader = DataLoader(test_data, batch_size=64, shuffle=False,
                          num_workers=2)




# Define the training function
def train_model(model, train_loader, optimizer, criterion, scheduler, device):
    model.train()
    running_loss = 0.0
    correct = 0
    total = 0

    for data, target in train_loader:
        data, target = data.to(device), target.to(device)
        optimizer.zero_grad()
        output = model(data)
        loss = criterion(output, target)
        loss.backward()
        optimizer.step()

        running_loss += loss.item() * data.size(0)
        _, predicted = torch.max(output, 1)
        total += target.size(0)
        correct += (predicted == target).sum().item()

    epoch_loss = running_loss / len(train_loader.dataset)
    accuracy = 100 * correct / total
    # Step the scheduler
    scheduler.step()
    return epoch_loss, accuracy


# Define the testing function
def test_model(model, test_loader, criterion, device):
    model.eval()
    running_loss = 0.0
    correct = 0
    total = 0

    with torch.no_grad():
        for data, target in test_loader:
            data, target = data.to(device), target.to(device)
            output = model(data)
            loss = criterion(output, target)
            running_loss += loss.item() * data.size(0)
            _, predicted = torch.max(output, 1)
            total += target.size(0)
            correct += (predicted == target).sum().item()
    epoch_loss = running_loss / len(test_loader.dataset)
    accuracy = 100 * correct / total
    return epoch_loss, accuracy

criterion = nn.CrossEntropyLoss()
optimizer = torch.optim.SGD(model.parameters(), lr=0.01, weight_decay=0.001, momentum = 0.9)
scheduler = StepLR(optimizer, step_size=10, gamma=0.1)

device = torch.device("cuda")

num_epochs = 20
for epoch in range(num_epochs):
    train_loss, train_accuracy = train_model(model, train_loader, optimizer, criterion, scheduler, device)
    test_loss, test_accuracy = test_model(model, test_loader, criterion, device)
    print(f'Epoch {epoch+1}/{num_epochs}, Train Loss: {train_loss:.4f}, Train Accuracy: {train_accuracy:.2f}%, Test Loss: {test_loss:.4f}, Test Accuracy: {test_accuracy:.2f}%')
