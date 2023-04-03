#include "Arduino.h"
#include <SoftwareSerial.h>
#include <Adafruit_NeoPixel.h>

//#define LED_PIN   5
#define LED_PIN   6
#define LED_COUNT 120

Adafruit_NeoPixel strip(LED_COUNT, LED_PIN, NEO_GRB + NEO_KHZ800);

void setLeds(String message);
String getNextNumber(String text, int cursor);

const byte rxPin = 9;
const byte txPin = 8; 
SoftwareSerial BTSerial(rxPin, txPin);

void setup() {
  pinMode(rxPin, INPUT);
  pinMode(txPin, OUTPUT);
  BTSerial.begin(9600);
  Serial.begin(9600);
  strip.begin();
  strip.show();
  pinMode(LED_BUILTIN, OUTPUT);
}
const char ACTIVATE_TOKEN = 'a';
bool WheelToken = false;

const char START_TOKEN = '?';
const char END_TOKEN = ';';
const char DELIMIT_TOKEN = '&';
const int CHAR_TIMEOUT = 20;

bool waitingForStartToken = true;
String messageBuffer = "";
String effect = "";

long lastRun = millis();
bool outputValue = false;

uint16_t i, j;

void loop() {
  char nextData;
  
  if (Serial.available()) {
    
    if (waitingForStartToken) {
      do {
        nextData = Serial.read();
      } while((nextData != START_TOKEN) && Serial.available());
      if (nextData == START_TOKEN) {
        Serial.println("message start");
        waitingForStartToken = false;
      }
    }

    if(!waitingForStartToken && Serial.available()) {
      do {
        nextData = Serial.read();
        Serial.println(nextData);
        messageBuffer += nextData;
      } while((nextData != END_TOKEN) && Serial.available());

      if (nextData == END_TOKEN) {
        messageBuffer = messageBuffer.substring(0, messageBuffer.length() - 1);
        Serial.println("message complete - " + messageBuffer);
        //?r=255&g=255&b=255&e=25&f=100;
        //setLeds(messageBuffer);

        //TODO: Read values for r, g, b, e and f
        
        effect = messageBuffer;
        //Serial.println(effect);
        messageBuffer = "";
        waitingForStartToken = true;
        WheelToken = false;
      }

//      if ((nextData ==  ACTIVATE_TOKEN) || (WheelToken == true)) {
//         Serial.println("Inside rainbow");
//         WheelToken = true;
//         rainbowCycle(10);
//         delay(1000);
//      }
       
//      if (messageBuffer.length() > CHAR_TIMEOUT) {
//        Serial.println("message data timeout - " + messageBuffer);
//        messageBuffer = "";
//        waitingForStartToken = true;
//        WheelToken = false;
//      }
    }
 
  }

  if (effect == "") {
  } else if (effect == "1") {
    //rainbowCycle
    for(i=0; i< strip.numPixels(); i++) {
      strip.setPixelColor(i, Wheel(((i * 256 / strip.numPixels()) + j) & 255));        
    }
    strip.show();

    if (j < 256) {
      j++;
    } else {
      j = 0;
    }
  } else if (effect ==  "2") {
    //rainbow
    for(i=0; i<strip.numPixels(); i++) {
      strip.setPixelColor(i, Wheel((i+j) & 255));
    }
    strip.show();

    if (j < 256) {
      j++;
    } else {
      j = 0;
    }
  } else {
    setLeds(effect);    
    effect = "";
  }

    delay(10);
}





void setLeds(String message) {
  int textCursor = 0;
  int pixel = 0;
  bool rgb0k = true;
  String red = "";
  String green = "";
  String blue = "";

 if (message.startsWith("r=")) {
  textCursor = 2;
  red = getNextNumber(message, textCursor);
  textCursor += red.length() + 1;
  message=message.substring(textCursor);
 }
 
 else {
  rgb0k = false;
 }

  if (message.startsWith("g=")) {
    textCursor = 2;
    green = getNextNumber(message, textCursor);
    textCursor += green.length() + 1;
    message=message.substring(textCursor);
  }

 else {
  rgb0k = false;
 }

  if (message.startsWith("b=")) {
    textCursor = 2;
    blue = getNextNumber(message, textCursor);
    textCursor += blue.length() + 1;
    message=message.substring(textCursor);
  }

 else {
  rgb0k = false;
 }
 
 if (rgb0k) {
  Serial.println("red = " + red);
  Serial.println("green = " + green);
  Serial.println("blue = " + blue);
  for(pixel = 0; pixel < LED_COUNT; pixel++) {
    strip.setPixelColor(pixel, red.toInt(), green.toInt(), blue.toInt());
  }
  strip.show();
 }
}

String getNextNumber(String text, int textCursor) {
  String number = "";
  while((text[textCursor] >= '0') && (text[textCursor] <= '9') && (textCursor < text.length())){
    number += text[textCursor];
    textCursor ++;
  }
  return number;
}

//Added 
void rainbowCycle(uint8_t wait) {
  uint16_t i, j;

  for(j=0; j<256*1; j++) { // 5 cycles of all colors on wheel
    for(i=0; i< strip.numPixels(); i++) {
      strip.setPixelColor(i, Wheel(((i * 256 / strip.numPixels()) + j) & 255));
    }
    strip.show();
    delay(wait);
  }
}

uint32_t Wheel(byte WheelPos) {
  WheelPos = 255 - WheelPos;
  if(WheelPos < 85) {
    return strip.Color(255 - WheelPos * 3, 0, WheelPos * 3);
  }
  if(WheelPos < 170) {
    WheelPos -= 85;
    return strip.Color(0, WheelPos * 3, 255 - WheelPos * 3);
  }
  WheelPos -= 170;
  return strip.Color(WheelPos * 3, 255 - WheelPos * 3, 0);
}
