#include <SoftwareSerial.h>  
#include <Adafruit_NeoPixel.h>


#define RxD 1
#define TxD 0

#define PIN1 3
#define PIN2 4

Adafruit_NeoPixel strip1 = Adafruit_NeoPixel(7, PIN1, NEO_GRB + NEO_KHZ800);
Adafruit_NeoPixel strip2 = Adafruit_NeoPixel(9, PIN2, NEO_GRB + NEO_KHZ800);

uint16_t i, j, effect;

#define DEBUG_ENABLED  1
 
SoftwareSerial blueToothSerial(RxD,TxD);

void setup() 
{ 
  pinMode(RxD, INPUT);
  pinMode(TxD, OUTPUT);
  blueToothSerial.begin(9600);

  strip1.begin();
  strip1.setBrightness(50);
  strip1.show();

  strip2.begin();
  strip2.setBrightness(50);
  strip2.show();
  
} 
 
void loop() 
{ 
  char recived;
  recived = blueToothSerial.read();

  if (blueToothSerial.available()) 
  {
    //ColorWipes
    if(recived=='r')
    {
      blueToothSerial.print("Red!\n");
      colorWipe(strip1.Color(127, 0, 0), 1); // Red
      ReversedcolorWipe(strip1.Color(0, 0, 0, 255), 50); // White RGBW
    }

    if(recived=='g')
    {
      blueToothSerial.print("Green!\n");
      colorWipe(strip1.Color(255, 0, 0), 1); // Green
      ReversedcolorWipe(strip1.Color(0, 0, 0, 255), 50); // White RGBW
    }

    if(recived=='b')
    {
      blueToothSerial.print("Blue!\n");
      colorWipe(strip1.Color(64, 64, 64), 1); // Blue
      ReversedcolorWipe(strip1.Color(0, 0, 0, 255), 50); // White RGBW
    }

    //PoliceMode
    if (recived=='p')
    {
      for (int i=0; i<1; i++) {
      blueToothSerial.print("Police!\n");
      solidColor1(strip1.Color(255,0,0),1);
      solidColor2(strip2.Color(0,0,255),1);
      // delay(500);
      // solidColor1(strip1.Color(0,0,255),1);
      // solidColor2(strip2.Color(255,0,0),1);
      // delay(500);
      }
      // delay(1000);
      // solidColor1(strip1.Color(0,0,0),1);
      // solidColor2(strip2.Color(0,0,0),1);
    }
   } 
  
  delay(100);
} 
 
void colorWipe(uint32_t c, uint8_t wait) 
{
  for(uint16_t i=6; i<strip1.numPixels(); i--) 
  {
    strip1.setPixelColor(i, c);
    strip1.show();
    delay(wait);
    delay(25);

  }
}

void ReversedcolorWipe(uint32_t c, uint8_t wait) 
{
  for(uint16_t i=0; i<strip1.numPixels(); i++) {
    strip1.setPixelColor(i, c);
    strip1.show();
    delay(wait);
    delay(25);

  }
}

void solidColor1(uint32_t c, uint8_t wait)
{
  for (uint16_t i=0; i<strip1.numPixels(); i++) {
    strip1.setPixelColor(i, c);
  }

    strip1.show();
    delay(wait);
    delay(25);
}

void solidColor2(uint32_t c, uint8_t wait)
{
  for (uint16_t i=0; i<strip2.numPixels(); i++) {
    strip2.setPixelColor(i, c);
  }

    strip2.show();
    delay(wait);
    delay(25);
}