#include <Adafruit_GFX.h>
#include "Adafruit_LEDBackpack.h"
#include "TimerOne.h"
#include <SD.h>

Adafruit_8x8matrix matrix[5];
const uint8_t MATRIX_EYEL = 0;
const uint8_t MATRIX_EYER = 1;
const uint8_t MATRIX_MOUTHL = 2;
const uint8_t MATRIX_MOUTHM = 3;
const uint8_t MATRIX_MOUTHR = 4;

String filename[5];

int remainingTime[5];

uint8_t frame[5];

bool updateMatrix[5];

File readingFile;

void setup() {
  //Init
  for(int i = 0; i < 5; i ++) {
    filename[i] = "";
    remainingTime[i] = 0;
    frame[i] = 0;
    updateMatrix[i] = false;
  }
  
  Serial.begin(9600);
  if(!SD.begin(4)) {
    Serial.println("SD initialization failed!");
    return;
  }
  Serial.println("Starting...");
  /*TODO: assign the rigth numbers*/
  matrix[MATRIX_MOUTHL].begin(0x70);
  matrix[MATRIX_MOUTHR].begin(0x71);
  matrix[MATRIX_MOUTHM].begin(0x72);
  matrix[MATRIX_EYER].begin(0x74);
  matrix[MATRIX_EYEL].begin(0x76);

  playAnimation("test1_", MATRIX_MOUTHM);
  playAnimation("test1_", MATRIX_MOUTHL);
  /*Run timeBase every 50 ms*/
  //Timer1.initialize(0.05 * 1000000);
  //Timer1.initialize(5 * 1000000);
  //Timer1.attachInterrupt(timeBase);
}

void playAnimation(String name, uint8_t m) {
  filename[m] = name;
  remainingTime[m] = 0;
  frame[m] = 0;
}

void timeBase() {/*
  Serial.println("timebase");
  for(int m = 0; m < 5; m ++) {
    if(!updateMatrix[m]) {
      if(remainingTime[m] == 0) {
        updateMatrix[m] = true;
      }
      remainingTime[m] --;
    }
  }*/
}

void loop() {/*
  for(int m = 0; m < 5; m ++) {
    if(updateMatrix[m]) {
      remainingTime[m] = loadFile(filename[m], frame[m], m);
      //File does not exist -> no more frame -> replay animation
      if(remainingTime[m] == -1) {
        frame[m] = 0;
        remainingTime[m] = loadFile(filename[m], frame[m], m);
        if(remainingTime[m] == -1) {
          //Error, requested file does not exist
          //Serial.println("File does not exist: " + filename[m]);
          //reckeck as soon as possible:
          remainingTime[m] = 1;
        }
      }
      frame[m] ++;
      remainingTime[m] --;
      updateMatrix[m] = false;
    }
  }
  */
  if(Serial.available() > 0) {
    char buf[80];
    Serial.readBytesUntil('\n', buf, 80);
    if(String(buf).startsWith("/a")) {
      Serial.println("A!");
      Serial.println(String(buf).substring(2, sizeof(buf)));
    }
  }
}

/*
String[] getCommandParts(String cmd, int max) {
  char myArray[cmd.size()+1];//as 1 char space for null is also required
  strcpy(myArray, cmd.c_str());
  String res[max];
  while(myArray[]) {
    
  }
}*/

int loadFile(String filename, uint8_t frame, uint8_t matrix_index) {
  readingFile = SD.open(filename + frame + ".txt");
  if(readingFile) {
    int duration = 0;
    uint8_t data[8];
    for(int i = 7; i >= 0; i --) {
      uint8_t nextchar = readingFile.read();
      if(nextchar == '0') {
        duration |= 0<<i;
      }
      else {
        duration |= 1<<i;
      }
    }
    for(uint8_t n = 0; n < 8; n ++) {
      data[n] = 0;
      for(int i = 7; i >= 0; i --) {
        uint8_t nextchar = readingFile.read();
        if(nextchar == '0') {
          data[n] |= 0<<i;
        }
        else {
          data[n] |= 1<<i;
        }
      }
    }
    readingFile.close();
    drawImage(data, matrix_index);
    return duration;
  }
  else {
    return -1;
  }
}

void drawImage(uint8_t data[], uint8_t m) {
  Serial.println("-----");
  for(int i = 0; i < 8; i ++) {
    Serial.println(data[i], BIN);
  }
  Serial.println("-----");
  matrix[m].clear();
  matrix[m].drawBitmap(0, 0, data, 8, 8, LED_ON);
  matrix[m].writeDisplay();
}

