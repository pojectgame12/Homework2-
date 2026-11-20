
# Question 2a - Orthographic Camera Explanation

When the phone is rotated from landscape to portrait,
the screen height in pixels increases.

If the camera's orthographicSize remains constant,
the same vertical area of the game world is displayed
over a larger number of pixels.

As a result, the number of pixels per world unit increases,
causing all game objects to appear larger on the screen.

To maintain a consistent object size, we must adjust the
camera's orthographicSize proportionally to Screen.height.

The relationship is:

orthographicSize = referenceSize * Screen.height / referenceHeight

This preserves the number of pixels per world unit
when the screen orientation changes.
