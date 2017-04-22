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

String filename_el = "b1_";
String filename_er = "";
String filename_ml = "";
String filename_mm = "";
String filename_mr = "";

int time_el = 0;
int time_er = 0;
int time_ml = 0;
int time_mm = 0;
int time_mr = 0;

uint8_t frame_el = 0;
uint8_t frame_er = 0;
uint8_t frame_ml = 0;
uint8_t frame_mm = 0;
uint8_t frame_mr = 0;

File readingFile;

void setup() {
  Serial.begin(9600);
  if(!SD.begin(4)) {
    Serial.println("SD initialization failed!");
    return;
  }
  /*TODO: assign the rigth numbers*/
  matrix[MATRIX_EYEL].begin(0x70);
  matrix[MATRIX_EYER].begin(0x71);
  matrix[MATRIX_MOUTHL].begin(0x72);
  matrix[MATRIX_MOUTHM].begin(0x73);
  matrix[MATRIX_MOUTHR].begin(0x74);

  /*Run timeBase every 50 ms*/
  //Timer1.initialize(0.05 * 1000000);
  Timer1.initialize(1 * 1000000);
  Timer1.attachInterrupt(timeBase);
}

void timeBase() {
  if(time_el == 0) {
    time_el = loadFile(filename_el, frame_el, MATRIX_EYEL);
    /*File does not exist -> no more frame -> replay animation*/
    if(time_el == -1) {
      frame_el = 0;
      time_el = loadFile(filename_el, frame_el, MATRIX_EYEL);
    }
    frame_el ++;
  }
  time_el --;
}

void loop() {
  // put your main code here, to run repeatedly:

}

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
    Serial.println(data[i]);
  }
  Serial.println("-----");
  /*
  matrix[m].clear();
  matrix[m].drawBitmap(0, 0, data, 8, 8, LED_ON);
  matrix[m].writeDisplay();*/
}

