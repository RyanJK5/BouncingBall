namespace BouncingBall;

public readonly struct SimulationRules(
    uint ballCount = 1, float initialSpeed = 5, float initialRadius = 20, float gravity = 40, float radiusIncrement = 0, float outerRadiusIncrement = 0, float speedIncrement = 0, 
    bool redraw = true
) {

    public readonly float Gravity = gravity;

    public readonly float InitialSpeed = initialSpeed;

    public readonly float InitialRadius = initialRadius;

    public readonly uint BallCount = ballCount;
    

    public readonly float RadiusIncrement = radiusIncrement;

    public readonly float OuterRadiusIncrement = outerRadiusIncrement;

    public readonly float SpeedIncrement = speedIncrement;

    public readonly bool Redraw = redraw;

    public SimulationRules() : this(ballCount:1) { }
}