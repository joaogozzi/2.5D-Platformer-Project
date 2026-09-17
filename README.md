# Sorting Visualizer

A step-by-step sorting algorithm visualizer built with **C++ and raylib**.

This project provides a visual and interactive way to understand how different sorting algorithms work by displaying comparisons, swaps, execution time, and the current state of the array.

The project was created as a programming and algorithms project focused on **C++, object-oriented programming, algorithm implementation, and visualization**.

## GIFs

<img width="1284" height="768" alt="Sorter" src="https://github.com/user-attachments/assets/a5ddbdc5-37c0-4076-b5f0-dda49992ea07" />

<img width="1284" height="768" alt="Sorter 2" src="https://github.com/user-attachments/assets/e17cdad9-ff1c-48b5-a21d-b7f93e4f948c" />

## Features

* Visual representation of sorting algorithms using bars
* Step-by-step sorting animation
* Real-time comparison and swap counters
* Sorting time display
* Adjustable sorting speed
* Pause and resume functionality
* Random array generation
* Switch between algorithms using keyboard shortcuts
* Every algorithm starts with the **same original array** when switching algorithms
* Visual indication of elements currently being compared
* Sorted elements turn **green** when they reach their final position
* Resizable window support

## Sorting Algorithms

The visualizer currently includes:

1. **Bubble Sort**
2. **Insertion Sort**
3. **Selection Sort**
4. **Merge Sort**
5. **Quick Sort**
6. **Bucket Sort**
7. **Heap Sort**
8. **Intro Sort**

All algorithms use a common interface, allowing them to be managed by the same visualizer.

## Controls

| Key     | Action                 |
| ------- | ---------------------- |
| `1`     | Bubble Sort            |
| `2`     | Insertion Sort         |
| `3`     | Selection Sort         |
| `4`     | Merge Sort             |
| `5`     | Quick Sort             |
| `6`     | Bucket Sort            |
| `7`     | Heap Sort              |
| `8`     | Intro Sort             |
| `SPACE` | Pause / Resume         |
| `R`     | Generate a new array   |
| `UP`    | Increase sorting speed |
| `DOWN`  | Decrease sorting speed |

## Visualization

Each bar represents one element of the array. The height of the bar corresponds to its value.

During the sorting process:

* **White** — Normal element
* **Comparison color** — Element currently being compared
* **Green** — Element that has reached its final sorted position

The interface also displays statistics such as:

```text
Algorithm: Quick Sort
Time: 3.421 s
Comparisons: 1234
Swaps: 567
Elements: 100
Delay: 0.05
```

## Architecture

The project uses an abstract `SortingAlgorithm` class as a common interface for all sorting algorithms.

Each algorithm implements:

```cpp
virtual void Start(std::vector<Element>& array) = 0;
virtual void Step() = 0;
virtual bool IsFinished() const = 0;

virtual uint64_t GetComparisons() const = 0;
virtual uint64_t GetSwaps() const = 0;

virtual const char* GetName() const = 0;
```

This allows the `Visualizer` to work with any sorting algorithm without needing to know its internal implementation.

For example:

```cpp
mAlgorithm->Step();
```

The visualizer simply asks the current algorithm to perform its next operation.

## Step-Based Sorting

Instead of running an entire sorting algorithm in a single function call, each algorithm is divided into small steps.

This allows the sorting process to be visualized as it executes:

```text
Start
  ↓
Compare
  ↓
Swap / Move
  ↓
Compare
  ↓
Swap / Move
  ↓
...
  ↓
Finished
```

This approach makes the behavior of each algorithm easier to observe and understand.

## Project Structure

```text
SortingVisualizer/
│
├── Element.h
├── SortingAlgorithm.h
│
├── BubbleSort.h
├── BubbleSort.cpp
│
├── InsertionSort.h
├── InsertionSort.cpp
│
├── SelectionSort.h
├── SelectionSort.cpp
│
├── MergeSort.h
├── MergeSort.cpp
│
├── QuickSort.h
├── QuickSort.cpp
│
├── BucketSort.h
├── BucketSort.cpp
│
├── HeapSort.h
├── HeapSort.cpp
│
├── IntroSort.h
├── IntroSort.cpp
│
├── Visualizer.h
├── Visualizer.cpp
│
└── main.cpp
```

## Technologies

* **C++**
* **raylib**
* Object-Oriented Programming
* STL containers
* Sorting algorithms
* Algorithm visualization
* Step-based algorithm execution

## Purpose

This project was created to strengthen my understanding of:

* Sorting algorithms
* Algorithm complexity
* C++ object-oriented programming
* Polymorphism
* State-based algorithm execution
* Data visualization
* Performance statistics
* Software architecture

It also serves as a portfolio project demonstrating my ability to implement algorithms and build an interactive visualization system around them.

## Future Improvements

Possible improvements for future versions include:

* Add more sorting algorithms
* Add algorithm complexity information to the UI
* Add a direct performance comparison mode
* Improve Bucket Sort visualization
* Improve Intro Sort implementation
* Add custom array sizes
* Add different visualization styles
* Add mouse interaction
* Add algorithm-specific explanations
* Add graphs comparing algorithm statistics

## Author

**João Gozzi**
Game Programmer / Game Programming Student

Focused on **C++, C#, Unity, Unreal Engine, gameplay programming, UI programming, and technical problem solving**.
