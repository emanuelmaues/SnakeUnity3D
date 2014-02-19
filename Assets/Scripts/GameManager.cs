using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : EnhancedBehaviour {

	[SerializeField]
	int numColumns = 16;

	[SerializeField]
	int numLines = 10;
	
	public enum CellType {
		SPECIAL, EMPTY, OCCUPIED, APPLE
	}

	public CellType[,] Grid {
		get;
		private set;
	}

	public List<Vector2> EmptyCells {
		get;
		private set;
	}

	/// <summary>
	/// My transform.
	/// </summary>
	Transform myTransform;

	public Transform MyTransform {
		get {
			if(!myTransform) 
				myTransform = transform;
			return myTransform;
		}
	}

	/// <summary>
	/// The root local scale.
	/// </summary>
	Vector3 rootLocalScale;

	/// <summary>
	/// Gets the grid bounds.
	/// </summary>
	/// <value>The grid bounds.</value>
	public Bounds GridBounds {
		get;
		private set;
	}

	/// <summary>
	/// Obtem o tamanho real do grid.
	/// NAO CHAMAR SEMPRE.
	/// </summary>
	/// <value>The grid bounds.</value>
	void CalculateBounds() {

		rootLocalScale = MyTransform.root.localScale;

		Bounds baseGridBounds = GetComponent<SpriteRenderer>().sprite.bounds;
		GridBounds = new Bounds(baseGridBounds.center, Vector3.Scale(baseGridBounds.size, MyTransform.lossyScale));
	}

	/// <summary>
	/// Convert a Grid position to local position.
	/// </summary>
	/// <returns>The to position.</returns>
	/// <param name="colIdx">Col index.</param>
	/// <param name="lineIdx">Line index.</param>
	public Vector3 GridToPosition(int colIdx, int lineIdx) {

		Vector3 position = Vector3.zero;

		position.x = Mathf.Lerp(-GridBounds.extents.x + GridBounds.extents.x / numColumns, 
		                         GridBounds.extents.x - GridBounds.extents.x / numColumns,(colIdx) / ((float) numColumns - 1));
		position.y = Mathf.Lerp(-GridBounds.extents.y + GridBounds.extents.y / numLines, 
		                         GridBounds.extents.y - GridBounds.extents.y / numLines, (lineIdx) / ((float) numLines - 1));
		position.z = 0f;

		return transform.TransformPoint(position);
	}

	protected override void EnhancedAwake ()
	{
		base.EnhancedAwake ();

		CalculateBounds();

		SetupArena();
		SetupSnake();
		StartInput();
	}

	/// <summary>
	/// The obstacle.
	/// </summary>
	[SerializeField]
	Transform obstacle;

	void SetupArena() {

		Grid = new CellType[numColumns, numLines];

		EmptyCells = new List<Vector2>();
		
		for (int i = 0; i < numColumns; i++) {
			for (int j = 0; j < numLines; j++) {
				EmptyCells.Add(new Vector2(i, j));
			}
		}

		obstacle.CreatePool();

		int[][] arenas = { ArenaManager.arena1, ArenaManager.arena2, ArenaManager.arena3,
			ArenaManager.arena4, ArenaManager.arena5, ArenaManager.arena6, ArenaManager.arena7 };

		int[] randomArena = arenas[Random.Range(0, arenas.Length)];

		for (int i = numLines - 1, s = 0; i >= 0; i--) {

			for (int j = 0; j < numColumns; j++, s++) {

				if(randomArena[s] == 1) {
					obstacle.Spawn(GridToPosition(j, i)).gameObject.SetActive(true);
					Grid[j, i] = CellType.OCCUPIED;
					EmptyCells.Remove(new Vector2(j, i));
				}
			}
		}
	}

	[SerializeField]
	SnakeBody bodyPrefab;

	/// <summary>
	/// The snake.
	/// </summary>
	List<SnakeBody> Snake {
		get;
		set;
	}
	
	enum Direction {
		UP, 
		DOWN, 
		LEFT, 
		RIGHT
	}

	/// <summary>
	/// The input direction.
	/// </summary>
	Direction inputDirection;

	/// <summary>
	/// The current direction.
	/// </summary>
	Direction curDirection = Direction.RIGHT;
	
	/// <summary>
	/// Setups the snake.
	/// </summary>
	void SetupSnake() {

		Snake = new List<SnakeBody>();

		bodyPrefab.CreatePool();

		//Spawn head
		SpawnSnakeBody(8, 5);
		SpawnSnakeBody(7, 5);
		SpawnSnakeBody(6, 5);

		// Reseta o input
		inputDirection = curDirection;
	}

	SnakeBody SpawnSnakeBody(int columnIndex, int lineIndex) {

		SnakeBody snakeBody = bodyPrefab.Spawn(GridToPosition(columnIndex, lineIndex));
		snakeBody.ColumnIndex = columnIndex;
		snakeBody.LineIndex = lineIndex;
		snakeBody.transform.parent = MyTransform;
		Snake.Add(snakeBody);
		
		Grid[columnIndex, lineIndex] = CellType.OCCUPIED;

		EmptyCells.Remove(new Vector2(columnIndex, lineIndex));

		return snakeBody;
	}

	/// <summary>
	/// Starts the input.
	/// </summary>
	void StartInput() {
		TouchKit.Instance.OnSwipe += HandleOnSwipe;
	}

	/// <summary>
	/// Stops the input.
	/// </summary>
	void StopInput() {
		if(TouchKit.Instance) {
			TouchKit.Instance.OnSwipe -= HandleOnSwipe;
		}
	}

	void HandleOnSwipe (TKSwipeRecognizer obj)
	{
		if(!firstInputHasOccured) {
			StartCoroutine(GameLoop());
			firstInputHasOccured = true;
		}

		TKSwipeDirection swipeDir = obj.completedSwipeDirection;

		if(swipeDir == TKSwipeDirection.Up && curDirection != Direction.DOWN) {
			inputDirection = Direction.UP;
		}
		else if(swipeDir == TKSwipeDirection.Down && curDirection != Direction.UP) {
			inputDirection = Direction.DOWN;
		}
		else if(swipeDir == TKSwipeDirection.Right && curDirection != Direction.LEFT) {
			inputDirection = Direction.RIGHT;
		}
		else if(swipeDir == TKSwipeDirection.Left && curDirection != Direction.RIGHT) {
			inputDirection = Direction.LEFT;
		}
	}

	/// <summary>
	/// The show score.
	/// </summary>
	[SerializeField]
	TextMesh showScore;

	/// <summary>
	/// The time interval.
	/// </summary>
	float timeInterval;

	/// <summary>
	/// The first input.
	/// </summary>
	bool firstInputHasOccured = false;

	/// <summary>
	/// The message.
	/// </summary>
	[SerializeField]
	GameObject message;

	/// <summary>
	/// The apple audio source.
	/// </summary>
	AudioSource appleAudioSource;

	/// <summary>
	/// The death.
	/// </summary>
	[SerializeField]
	AudioClip deathClip;

	/// <summary>
	/// Moves the snake.
	/// </summary>
	/// <returns>The snake.</returns>
	IEnumerator GameLoop() {

		AudioSource appleEatenNoise = apple.GetComponent<AudioSource>();

		bool gameOver = false;
		bool appleEaten = false;

		int difficulty = Random.Range(1, 10);
		timeInterval = Mathf.Lerp(0.5f, 0.05f, Mathf.InverseLerp(1, 10, difficulty));

		int score = 0;

		SpawnApple();

		message.SetActive(false);

		while(!gameOver) {

			yield return new WaitForSeconds(timeInterval);

			Snake[Snake.Count - 1].transform.localScale = bodyPrefab.transform.localScale;

			curDirection = inputDirection;

			SnakeBody snakeHead = Snake[0];
			
			int colIdx = snakeHead.ColumnIndex;
			int lineIdx = snakeHead.LineIndex;
			EvaluateNextPosition(ref colIdx, ref lineIdx);

			if(Grid[colIdx, lineIdx] == CellType.APPLE) {
				score += 10 * difficulty;
				showScore.text = "SCORE: " + score.ToString();
				appleEaten = true;
				
				appleEatenNoise.Play();
			}

			// Checa game over.
			if(Grid[colIdx, lineIdx] == CellType.OCCUPIED) {

				SnakeBody tail = Snake[Snake.Count - 1];

				if(tail.ColumnIndex != colIdx || tail.LineIndex != lineIdx) {
					gameOver = true;
					break;
				}
			}

			bool isTail = true;

			// Move o corpo
			for (int i = Snake.Count - 1; i > 0; i--) {
				
				SnakeBody curSnakeBody = Snake[i];
				SnakeBody nextSnakeBody = Snake[i - 1];
				
				if(isTail) {
					
					if(appleEaten) {
						SnakeBody tail = SpawnSnakeBody(curSnakeBody.ColumnIndex, curSnakeBody.LineIndex);
						Grid[tail.ColumnIndex, tail.LineIndex] = CellType.OCCUPIED;
					}
					else {
						Grid[curSnakeBody.ColumnIndex, curSnakeBody.LineIndex] = CellType.EMPTY;
						EmptyCells.Add(new Vector2(curSnakeBody.ColumnIndex, curSnakeBody.LineIndex));
					}
					
					isTail = false;
				}
				
				curSnakeBody.ColumnIndex = nextSnakeBody.ColumnIndex;
				curSnakeBody.LineIndex = nextSnakeBody.LineIndex;
				curSnakeBody.transform.position = GridToPosition(curSnakeBody.ColumnIndex, curSnakeBody.LineIndex);
			}
			
			// Move a cabeça
			snakeHead.ColumnIndex = colIdx;
			snakeHead.LineIndex = lineIdx;
			snakeHead.transform.position = GridToPosition(snakeHead.ColumnIndex, snakeHead.LineIndex);
			Grid[snakeHead.ColumnIndex, snakeHead.LineIndex] = CellType.OCCUPIED;
			
			EmptyCells.Remove(new Vector2(colIdx, lineIdx));
			
			if(appleEaten) {
				
				apple.particleSystem.Play();
				yield return null;
				
				SpawnApple();
				appleEaten = false;
			}


		}

		// Game Over
		StopInput();

		PersistenceManager.Instance.LastScore = score;

		AudioSource aSource = GetComponent<AudioSource>();
		aSource.Stop();
		aSource.clip = deathClip;
		aSource.loop = false;

		yield return new WaitForSeconds(1f);

		for (int i = 0; i < Snake.Count; i++) {

			ParticleSystem pSys = Snake[i].particleSystem;
			pSys.Play();
			aSource.Play();

			yield return StartCoroutine(Snake[i].transform.ScaleTo(Vector3.zero, pSys.duration, EaseType.CubeInOut));

			Snake[i].Recycle();
		}

		yield return new WaitForSeconds(2f);
		Application.LoadLevel("gameover");
	}

	protected override void EnhancedOnDestroy ()
	{
		base.EnhancedOnDestroy ();
		StopInput();
	}

	[SerializeField]
	Transform apple;

	/// <summary>
	/// Spawns the apple.
	/// </summary>
	/// <returns>The apple.</returns>
	void SpawnApple() {
	
		int rndIndex = Random.Range(0, EmptyCells.Count);
		int columnIndex = Mathf.RoundToInt(EmptyCells[rndIndex].x);
		int lineIndex = Mathf.RoundToInt(EmptyCells[rndIndex].y);

		Grid[columnIndex, lineIndex] = CellType.APPLE;

		apple.gameObject.SetActive(true);
		apple.position = GridToPosition(columnIndex, lineIndex);

		StartCoroutine(apple.ScaleFrom(Vector3.zero, timeInterval, EaseType.CubeInOut));
	}
	
	/// <summary>
	/// Evaluates the next position.
	/// </summary>
	/// <param name="columnIndex">Column index.</param>
	/// <param name="lineIndex">Line index.</param>
	void EvaluateNextPosition(ref int columnIndex, ref int lineIndex) {

		switch (curDirection) {
			case Direction.UP:
				lineIndex++;
				break;
			case Direction.DOWN:
				lineIndex--;
				break;
			case Direction.RIGHT:
				columnIndex++;
				break;
			default:
				columnIndex--;
				break;
		}

		if(lineIndex < 0) {
			lineIndex = numLines - 1;
		}
		else if(lineIndex >= numLines) {
			lineIndex = 0;
		}

		if(columnIndex < 0) {
			columnIndex = numColumns - 1;
		}
		else if(columnIndex >= numColumns) {
			columnIndex = 0;
		}
	}

	protected override void EnhancedUpdate ()
	{
		base.EnhancedUpdate ();

		Vector3 curRootLocalScale = MyTransform.root.localScale;

		if(rootLocalScale != curRootLocalScale) {
			CalculateBounds();
		}
	}
}

public class ArenaManager {

	public static int[] arena1 = {
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
	};

	public static int[] arena2 = {
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
	};

	public static int[] arena3 = {
		1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1
	};

	public static int[] arena4 = {
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0
	};

	public static int[] arena5 = {
		1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0,
		1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0
	};

	public static int[] arena6 = {
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
		0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
		0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
		0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
	};

	public static int[] arena7 = {
		0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1,
		0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
		1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0
	};
}