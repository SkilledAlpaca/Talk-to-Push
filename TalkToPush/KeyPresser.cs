using System;
using System.Windows.Forms;

namespace TalkToPush
{
    class KeyPresser
    {
        private Robot robot;
        private Keys key;
        private bool isKeyDown;


        public KeyPresser() {
            this.robot = new Robot();
        }

        public void setKey(Keys key) {
            bool wasDown = false;

            if (isKeyDown) {
                wasDown = true;
                release();
            }

            this.key = key;

            if (wasDown) {
                beginHold();
            }
        }

        public Keys getKey() {
            return key;
        }

        public bool beginHold() {
            if (!isKeyDown) {
                try {
                    robot.keyPress(key);
                    isKeyDown = true;
                    return true;
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
            return false;
        }

        public bool release() {

            if (isKeyDown) {

                try {
                    robot.keyRelease(key);
                    isKeyDown = false;
                    return true;
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
            return false;
        }

        public bool isPressing() {
            return isKeyDown;
        }
    }
}
