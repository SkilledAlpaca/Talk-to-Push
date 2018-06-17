import java.awt.*;
import java.util.Scanner;

public class Main {

    public static void main(String[] args) throws AWTException {

        Robot robot = new Robot();
        Scanner sc = new Scanner(System.in);

        while (true) {

            int command = sc.nextInt(); // Read the first number on this new line of standard input

            if (command == -1) {
                return;
            }

            int code = sc.nextInt(); // Read the second number. This is a key code, e.g. 90 = Z

            if (command == 0) {
                robot.keyRelease(code);
            } else {
                robot.keyPress(code); // Send key to the focused application
            }

            sc.nextLine(); // Ignore any garbage remaining on the line

        }
    }
}
