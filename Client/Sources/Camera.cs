using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class Camera
{
    private GraphicsDevice graphicsDevice;
    private int mapWidth, mapHeight;

    // Позиция камеры (в мировых координатах)
    public Vector2 Position { get; private set; } = Vector2.Zero;

    // Целевая позиция (для плавного движения)
    private Vector2 targetPosition;

    // Текущая скорость (для SmoothDamp)
    private Vector2 currentVelocity;

    // Время сглаживания (сек) – регулирует инерцию
    private float smoothTime = 0.3f;

    // Масштаб (zoom)
    private float zoom = 1.0f;
    private float minZoom = 0.2f;
    private float maxZoom = 5.0f;

    public float Zoom
    {
        get => zoom;
        set
        {
            zoom = MathHelper.Clamp(value, minZoom, maxZoom);
            UpdateMatrix(); // сразу обновляем матрицу при изменении масштаба
        }
    }

    public Matrix ViewMatrix { get; private set; } = Matrix.Identity;

    public Camera(GraphicsDevice graphicsDevice, int mapWidth, int mapHeight)
    {
        this.graphicsDevice = graphicsDevice;
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        targetPosition = Position;
    }

    /// <summary>
    /// Перемещает целевую точку камеры на указанное смещение в мировых координатах.
    /// </summary>
    public void Move(Vector2 worldDelta)
    {
        targetPosition += worldDelta;
        ClampTargetPosition();
    }

    /// <summary>
    /// Перемещает камеру так, чтобы на экране смещение составило screenDelta пикселей.
    /// Удобно для обработки ввода с клавиатуры (чтобы скорость прокрутки не зависела от зума).
    /// </summary>
    public void MoveScreen(Vector2 screenDelta)
    {
        // Преобразуем экранное смещение в мировое с учётом масштаба
        Vector2 worldDelta = screenDelta / zoom;
        Move(worldDelta);
    }

    public void ZoomIn(float delta) => Zoom += delta;
    public void ZoomOut(float delta) => Zoom -= delta;

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Плавное движение к целевой позиции
        Position = SmoothDamp(
            Position,
            targetPosition,
            ref currentVelocity,
            smoothTime,
            float.PositiveInfinity,
            deltaTime
        );

        // Финальное ограничение (на случай, если цель была вне границ)
        ClampPositionToBounds();
        UpdateMatrix();
    }

    // ---------- Вспомогательные методы ----------

    private void ClampTargetPosition()
    {
        float worldScreenWidth = graphicsDevice.Viewport.Width / zoom;
        float worldScreenHeight = graphicsDevice.Viewport.Height / zoom;

        float maxX = Math.Max(0, mapWidth - worldScreenWidth);
        float maxY = Math.Max(0, mapHeight - worldScreenHeight);

        targetPosition.X = MathHelper.Clamp(targetPosition.X, 0, maxX);
        targetPosition.Y = MathHelper.Clamp(targetPosition.Y, 0, maxY);
    }

    private void ClampPositionToBounds()
    {
        float worldScreenWidth = graphicsDevice.Viewport.Width / zoom;
        float worldScreenHeight = graphicsDevice.Viewport.Height / zoom;

        float maxX = Math.Max(0, mapWidth - worldScreenWidth);
        float maxY = Math.Max(0, mapHeight - worldScreenHeight);

        float clampedX = MathHelper.Clamp(Position.X, 0, maxX);
        float clampedY = MathHelper.Clamp(Position.Y, 0, maxY);
        Position = new Vector2(clampedX, clampedY);
    }

    private void UpdateMatrix()
    {
        // Сначала смещение, потом масштаб
        ViewMatrix = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                     Matrix.CreateScale(zoom);
    }

    // Реализация SmoothDamp (адаптировано из Unity)
    private Vector2 SmoothDamp(
        Vector2 current,
        Vector2 target,
        ref Vector2 velocity,
        float smoothTime,
        float maxSpeed,
        float deltaTime)
    {
        float omega = 2f / smoothTime;
        float x = omega * deltaTime;
        float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

        Vector2 change = current - target;
        Vector2 originalTarget = target;

        float maxChange = maxSpeed * smoothTime;
        change.X = MathHelper.Clamp(change.X, -maxChange, maxChange);
        change.Y = MathHelper.Clamp(change.Y, -maxChange, maxChange);

        target = current - change;

        Vector2 temp = (velocity + omega * change) * deltaTime;
        velocity = (velocity - omega * temp) * exp;
        Vector2 output = target + (change + temp) * exp;

        // Предотвращаем перелёт через цель
        if (Vector2.Dot(originalTarget - current, output - originalTarget) > 0)
        {
            output = originalTarget;
            velocity = (output - originalTarget) / deltaTime;
        }

        return output;
    }
}