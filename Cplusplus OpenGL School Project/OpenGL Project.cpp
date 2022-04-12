#include <iostream>         // cout, cerr
#include <cstdlib>          // EXIT_FAILURE
#include <GL/glew.h>        // GLEW library
#include <GLFW/glfw3.h>     // GLFW library
#define STB_IMAGE_IMPLEMENTATION
#include <stb_image.h>      // Image loading Utility functions

// GLM Math Header inclusions
#include <glm/glm.hpp>
#include <glm/gtx/transform.hpp>
#include <glm/gtc/type_ptr.hpp>

#include <camera.h>

using namespace std; // Standard namespace

/*Shader program Macro*/
#ifndef GLSL
#define GLSL(Version, Source) "#version " #Version " core \n" #Source
#endif

// Unnamed namespace
namespace
{
    const char* const WINDOW_TITLE = "Joseph Helmuth 7-1 Final Project"; // Macro for window title

    // Variables for window width and height
    const int WINDOW_WIDTH = 800;
    const int WINDOW_HEIGHT = 600;

    // Stores the GL data relative to a given mesh
    struct GLMesh
    {
        GLuint vao;         // Handle for the vertex array object
        // Tried changing vbos[2] to just vbo. but the rest did not work correctly after so I reverted code
        GLuint vbos[2];     // Handles for the vertex buffer objects
        /* Tried changing to just using nVertices instead of nIndices and it failed, more code in UCreateMesh trying to get it to work properly
        GLuint nVertices;    // Number of indices of the mesh */
        GLuint nIndices;    // Number of indices of the mesh
    };

    // Main GLFW window
    GLFWwindow* gWindow = nullptr;
    // Triangle mesh data
    GLMesh gMesh;
    // Texture id
    GLuint gTextureId;
    // Shader program
    GLuint gProgramId;

    // Camera
    Camera gCamera(glm::vec3(0.0f, 0.0f, 3.0f));
    glm::vec3 gCameraPos = glm::vec3(0.0f, 0.0f, 3.0f);
    glm::vec3 gCameraFront = glm::vec3(0.0f, 0.0f, -1.0f);
    glm::vec3 gCameraUp = glm::vec3(0.0f, 1.0f, 0.0f);
    glm::vec3 gCameraRight = glm::vec3(1.0f, 0.0f, 0.0f);
    float gLastX = WINDOW_WIDTH / 2.0f;
    float gLastY = WINDOW_HEIGHT / 2.0f;
    bool gFirstMouse = true;

    // Timing
    float gDeltaTime = 0.0f; // Time between current frame and last frame
    float gLastFrame = 0.0f;

    // Subject position and scale
    glm::vec3 gCubePosition(0.0f, 0.0f, 0.0f);
    glm::vec3 gCubeScale(2.0f);

    // Cube and light color
    //glm::vec3 gObjectColor(0.6f, 0.5f, 0.75f);
    glm::vec3 gObjectColor(1.f, 0.2f, 0.0f);
    glm::vec3 gLightColor(1.0f, 1.0f, 1.0f);

    // Light position and scale
    glm::vec3 gLightPosition(1.5f, 5.5f, 5.0f);
    //glm::vec3 gLightPosition(1.5f, 0.5f, 3.0f);
    glm::vec3 gLightScale(0.3f);
}

/* User-defined Function prototypes to:
 * initialize the program, set the window size,
 * redraw graphics on the window when resized,
 * and render graphics on the screen
 */
bool UInitialize(int, char* [], GLFWwindow** window);
void UResizeWindow(GLFWwindow* window, int width, int height);
void UProcessInput(GLFWwindow* window);
void UMousePositionCallback(GLFWwindow* window, double xpos, double ypos);
void UMouseScrollCallback(GLFWwindow* window, double xoffset, double yoffset);
void UMouseButtonCallback(GLFWwindow* window, int button, int action, int mods);
bool UCreateTexture(const char* filename, GLuint& textureId);
void UDestroyTexture(GLuint textureId);
void UCreateMesh(GLMesh& mesh);
void UDestroyMesh(GLMesh& mesh);
void URender();
bool UCreateShaderProgram(const char* vtxShaderSource, const char* fragShaderSource, GLuint& programId);
void UDestroyShaderProgram(GLuint programId);


/* Vertex Shader Source Code*/
const GLchar* vertexShaderSource = GLSL(440,
    layout(location = 0) in vec3 position; // Vertex data from Vertex Attrib Pointer 0
    layout(location = 1) in vec3 normal;  // VAP position 1 for normals
    layout(location = 2) in vec2 textureCoordinate;  // Texture data from Vertex Attrib Pointer 1

    //out vec4 vertexColor; // variable to transfer color data to the fragment shader
    out vec3 vertexNormal; // For outgoing normals to fragment shader
    out vec3 vertexFragmentPos; // For outgoing color / pixels to fragment shader
    out vec2 vertexTextureCoordinate;  // variable to transfer Texture data to the fragment shader

    //Global variables for the  transform matrices
    uniform mat4 model;
    uniform mat4 view;
    uniform mat4 projection;

void main()
{
    gl_Position = projection * view * model * vec4(position, 1.0f); // transforms vertices to clip coordinates
    
    vertexFragmentPos = vec3(model * vec4(position, 1.0f)); // Gets fragment / pixel position in world space only (exclude view and projection)

    vertexNormal = mat3(transpose(inverse(model))) * normal; // get normal vectors in world space only and exclude normal translation properties

    vertexTextureCoordinate = textureCoordinate;
    //vertexColor = color; // references incoming color data
}
);


/* Fragment Shader Source Code*/
const GLchar* fragmentShaderSource = GLSL(440,
    in vec3 vertexNormal; // For incoming normals
    in vec3 vertexFragmentPos; // For incoming fragment position
    in vec2 vertexTextureCoordinate;

    out vec4 fragmentColor;

    // Uniform / Global variables for object color, light color, light position, and camera/view position
    uniform vec3 objectColor;
    uniform vec3 lightColor;
    uniform vec3 lightPos;
    uniform vec3 viewPosition;
    uniform sampler2D uTexture;
    uniform vec2 uvScale;

void main()
{
    fragmentColor = texture(uTexture, vertexTextureCoordinate); // Sends texture to the GPU for rendering

    /* Phong lighting not working
    //Phong lighting model calculations to generate ambient, diffuse, and specular components

    //Calculate Ambient lighting
    float ambientStrength = 0.1f; // Set ambient or global lighting strength
    vec3 ambient = ambientStrength * lightColor; // Generate ambient light color

    //Calculate Diffuse lighting
    vec3 norm = normalize(vertexNormal); // Normalize vectors to 1 unit
    vec3 lightDirection = normalize(lightPos - vertexFragmentPos); // Calculate distance (light direction) between light source and fragments/pixels on cube
    float impact = max(dot(norm, lightDirection), 0.0); // Calculate diffuse impact by generating dot product of normal and light
    vec3 diffuse = impact * lightColor; // Generate diffuse light color

    //Calculate Specular lighting
    float specularIntensity = 0.8f; // Set specular light strength
    float highlightSize = 16.0f; // Set specular highlight size
    vec3 viewDir = normalize(viewPosition - vertexFragmentPos); // Calculate view direction
    vec3 reflectDir = reflect(-lightDirection, norm); // Calculate reflection vector

    //Calculate specular component
    float specularComponent = pow(max(dot(viewDir, reflectDir), 0.0), highlightSize);
    vec3 specular = specularIntensity * specularComponent * lightColor;

    // Texture holds the color to be used for all three components
    vec4 textureColor = texture(uTexture, vertexTextureCoordinate * uvScale);

    // Calculate phong result
    vec3 phong = (ambient + diffuse + specular) * textureColor.xyz;

    // This did not work
    //fragmentColor = vec4(phong, 1.0); // Send lighting results to GPU
    */  
} 
); 

/* This did not work
// Vertex Shader Source Code
const GLchar* vertexShaderSource = GLSL(440,
    layout(location = 0) in vec3 position; // Vertex data from Vertex Attrib Pointer 0
layout(location = 1) in vec3 normal;  // Normal data from Vertex Attrib Pointer 1
layout(location = 2) in vec2 textureCoordinate;  // Texture data from Vertex Attrib Pointer 1

//out vec4 vertexColor; // variable to transfer color data to the fragment shader
out vec3 vertexNormal; // For outgoing normals to fragment shader
out vec3 vertexFragmentPos; // For outgoing color / pixels to fragment shader
out vec2 vertexTextureCoordinate;  // variable to transfer Texture data to the fragment shader

//Global variables for the  transform matrices
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = projection * view * model * vec4(position, 1.0f); // transforms vertices to clip coordinates

    vertexFragmentPos = vec3(model * vec4(position, 1.0f)); // Gets fragment / pixel position in world space only (exclude view and projection)

    vertexNormal = mat3(transpose(inverse(model))) * normal; // get normal vectors in world space only and exclude normal translation properties
    vertexTextureCoordinate = textureCoordinate;
    //vertexColor = color; // references incoming color data
}
); */

/* Did not work
// Lamp Shader Source Code
const GLchar* lampVertexShaderSource = GLSL(440,

    layout(location = 0) in vec3 position; // VAP position 0 for vertex position data

    //Uniform / Global variables for the  transform matrices
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = projection * view * model * vec4(position, 1.0f); // Transforms vertices into clip coordinates
}
); */

/* Did not work
// Fragment Shader Source Code
const GLchar* lampFragmentShaderSource = GLSL(440,

    out vec4 fragmentColor; // For outgoing lamp color (smaller cube) to the GPU

void main()
{
    fragmentColor = vec4(1.0f); // Set color to white (1.0f,1.0f,1.0f) with alpha 1.0
}
); */

/* Did not work
// Fragment Shader Source Code
const GLchar* fragmentShaderSource = GLSL(440,
    in vec2 vertexTextureCoordinate;

out vec4 fragmentColor;

uniform sampler2D uTexture;

void main()
{
    fragmentColor = texture(uTexture, vertexTextureCoordinate); // Sends texture to the GPU for rendering
}
); */

// Images are loaded with Y axis going down, but OpenGL's Y axis goes up, so let's flip it
void flipImageVertically(unsigned char* image, int width, int height, int channels)
{
    for (int j = 0; j < height / 2; ++j)
    {
        int index1 = j * width * channels;
        int index2 = (height - 1 - j) * width * channels;

        for (int i = width * channels; i > 0; --i)
        {
            unsigned char tmp = image[index1];
            image[index1] = image[index2];
            image[index2] = tmp;
            ++index1;
            ++index2;
        }
    }
}

int main(int argc, char* argv[])
{
    if (!UInitialize(argc, argv, &gWindow))
        return EXIT_FAILURE;

    // Create the mesh
    UCreateMesh(gMesh); // Calls the function to create the Vertex Buffer Object

    // Create the shader program
    if (!UCreateShaderProgram(vertexShaderSource, fragmentShaderSource, gProgramId))
        return EXIT_FAILURE;

    const char* texFilename = "resources/textures/Combo.png";
    if (!UCreateTexture(texFilename, gTextureId))
    {
        cout << "Failed to load texture " << texFilename << endl;
        return EXIT_FAILURE;
    }
    // Tell OpenGL for each sampler which texture unit it belongs to (only has to be done once).
    glUseProgram(gProgramId);
    // We set the texture as texture unit 0.
    glUniform1i(glGetUniformLocation(gProgramId, "uTexture"), 0);


    // Sets the background color of the window to black (it will be implicitely used by glClear)
    glClearColor(0.0f, 0.0f, 0.0f, 1.0f);

    // render loop
    // -----------
    while (!glfwWindowShouldClose(gWindow))
    {
        // Per frame timing
        float currentFrame = glfwGetTime();
        gDeltaTime = currentFrame - gLastFrame;
        gLastFrame = currentFrame;

        // input
        // -----
        UProcessInput(gWindow);

        // Render this frame
        URender();

        glfwPollEvents();
    }

    // Release mesh data
    UDestroyMesh(gMesh);

    // Release shader program
    UDestroyShaderProgram(gProgramId);
    //UDestroyShaderProgram(gCubeProgramId);
    //UDestroyShaderProgram(gLampProgramId);

    exit(EXIT_SUCCESS); // Terminates the program successfully
}


// Initialize GLFW, GLEW, and create a window
bool UInitialize(int argc, char* argv[], GLFWwindow** window)
{
    // GLFW: initialize and configure
    // ------------------------------
    glfwInit();
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 4);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

#ifdef __APPLE__
    glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
#endif

    // GLFW: window creation
    // ---------------------
    * window = glfwCreateWindow(WINDOW_WIDTH, WINDOW_HEIGHT, WINDOW_TITLE, NULL, NULL);
    if (*window == NULL)
    {
        std::cout << "Failed to create GLFW window" << std::endl;
        glfwTerminate();
        return false;
    }
    glfwMakeContextCurrent(*window);
    glfwSetFramebufferSizeCallback(*window, UResizeWindow);
    glfwSetCursorPosCallback(*window, UMousePositionCallback);
    glfwSetScrollCallback(*window, UMouseScrollCallback);
    glfwSetMouseButtonCallback(*window, UMouseButtonCallback);

    // GLEW: initialize
    // ----------------
    // Note: if using GLEW version 1.13 or earlier
    glewExperimental = GL_TRUE;
    GLenum GlewInitResult = glewInit();

    if (GLEW_OK != GlewInitResult)
    {
        std::cerr << glewGetErrorString(GlewInitResult) << std::endl;
        return false;
    }

    // Displays GPU OpenGL version
    cout << "INFO: OpenGL Version: " << glGetString(GL_VERSION) << endl;

    return true;
}

// process all input: query GLFW whether relevant keys are pressed/released this frame and react accordingly
void UProcessInput(GLFWwindow* window)
{
    static const float cameraSpeed = 2.5f;

    if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
        glfwSetWindowShouldClose(window, true);

    if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
        gCamera.ProcessKeyboard(FORWARD, gDeltaTime);
    if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
        gCamera.ProcessKeyboard(BACKWARD, gDeltaTime);
    if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
        gCamera.ProcessKeyboard(LEFT, gDeltaTime);
    if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
        gCamera.ProcessKeyboard(RIGHT, gDeltaTime);
    if (glfwGetKey(window, GLFW_KEY_Q) == GLFW_PRESS)
        gCamera.ProcessKeyboard(UP, gDeltaTime);
    if (glfwGetKey(window, GLFW_KEY_E) == GLFW_PRESS)
        gCamera.ProcessKeyboard(DOWN, gDeltaTime);
    //if (glfwGetKey(window, GLFW_KEY_P) == GLFW_PRESS)
        //UResizeWindow(0, WINDOW_WIDTH, WINDOW_HEIGHT);   // Tried some combonation of these things to switch to 2D view. Nothing worked though.
        //glMatrixMode(GL_PROJECTION);
        //glLoadIdentity();
        //glOrtho(0.0f, WINDOW_WIDTH, WINDOW_HEIGHT, 0.0f, 0.0f, 1.0f);
}

// glfw: whenever the mouse moves, this callback is called
// -------------------------------------------------------
void UMousePositionCallback(GLFWwindow* window, double xpos, double ypos)
{
    if (gFirstMouse)
    {
        gLastX = xpos;
        gLastY = ypos;
        gFirstMouse = false;
    }

    float xoffset = xpos - gLastX;
    float yoffset = gLastY - ypos; // reversed since y-coordinates go from bottom to top

    gLastX = xpos;
    gLastY = ypos;

    gCamera.ProcessMouseMovement(xoffset, yoffset);
}


// glfw: whenever the mouse scroll wheel scrolls, this callback is called
// ----------------------------------------------------------------------
void UMouseScrollCallback(GLFWwindow* window, double xoffset, double yoffset)
{
    gCamera.ProcessMouseScroll(yoffset);
}

// glfw: handle mouse button events
// --------------------------------
void UMouseButtonCallback(GLFWwindow* window, int button, int action, int mods)
{
    switch (button)
    {
    case GLFW_MOUSE_BUTTON_LEFT:
    {
        if (action == GLFW_PRESS)
            cout << "Left mouse button pressed" << endl;
        else
            cout << "Left mouse button released" << endl;
    }
    break;

    case GLFW_MOUSE_BUTTON_MIDDLE:
    {
        if (action == GLFW_PRESS)
            cout << "Middle mouse button pressed" << endl;
        else
            cout << "Middle mouse button released" << endl;
    }
    break;

    case GLFW_MOUSE_BUTTON_RIGHT:
    {
        if (action == GLFW_PRESS)
            cout << "Right mouse button pressed" << endl;
        else
            cout << "Right mouse button released" << endl;
    }
    break;

    default:
        cout << "Unhandled mouse button event" << endl;
        break;
    }
}

// glfw: whenever the window size changed (by OS or user resize) this callback function executes
void UResizeWindow(GLFWwindow* window, int width, int height)
{
    glViewport(0, 0, width, height);
    //glm::mat4 projection = glm::ortho(0.0f, 800.0f, 600.0f, 0.0f, -1.0f, 1.0f);
}


// Functioned called to render a frame
void URender()
{
    // Enable z-depth
    glEnable(GL_DEPTH_TEST);

    // Clear the frame and z buffers
    glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

    // Activate the cube VAO (used by cube and lamp)
    glBindVertexArray(gMesh.vao);

    /* Did not work, had to revert to previous code
    // CUBE: draw cube
    //----------------
    // Set the shader to be used
    glUseProgram(gCubeProgramId);

    // Model matrix: transformations are applied right-to-left order
    glm::mat4 model = glm::translate(gCubePosition) * glm::scale(gCubeScale); */

    // 1. Scales the object by 2
    glm::mat4 scale = glm::scale(glm::vec3(2.0f, 2.0f, 2.0f));
    // 2. Rotates shape by some degrees in the x axis
    glm::mat4 rotation = glm::rotate(0.5f, glm::vec3(0.0, 0.0f, 1.0f));

    //glm::mat4 rotation = glm::rotate(71.7f, glm::vec3(1.0, 1.0f, 1.0f));
    // 3. Place object at the origin
    glm::mat4 translation = glm::translate(glm::vec3(0.0f, 0.0f, -5.0f));
    // Model matrix: transformations are applied right-to-left order
    glm::mat4 model = translation * rotation * scale;

    // camera/view transformation
    glm::mat4 view = gCamera.GetViewMatrix();
    glm::mat4 projection = glm::perspective(glm::radians(gCamera.Zoom), (GLfloat)WINDOW_WIDTH / (GLfloat)WINDOW_HEIGHT, 0.1f, 100.0f);

    // Transforms the camera: move the camera back (z axis)
    //glm::mat4 view = glm::translate(glm::vec3(0.0f, 0.0f, -5.0f));

    // Creates a orthographic projection
    //glm::mat4 projection = glm::ortho(-5.0f, 5.0f, -5.0f, 5.0f, 0.1f, 100.0f);

    // Set the shader to be used
    glUseProgram(gProgramId);

    glActiveTexture(GL_TEXTURE0);
    glBindTexture(GL_TEXTURE_2D, gTextureId);

    // Retrieves and passes transform matrices to the Shader program
    GLint modelLoc = glGetUniformLocation(gProgramId, "model");
    GLint viewLoc = glGetUniformLocation(gProgramId, "view");
    GLint projLoc = glGetUniformLocation(gProgramId, "projection");

    glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));
    glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));
    glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));

    // Activate the VBOs contained within the mesh's VAO
    glBindVertexArray(gMesh.vao);

    /* Did not work, reverted to previous code
    // Reference matrix uniforms from the Cube Shader program for the cub color, light color, light position, and camera position
    GLint objectColorLoc = glGetUniformLocation(gCubeProgramId, "objectColor");
    GLint lightColorLoc = glGetUniformLocation(gCubeProgramId, "lightColor");
    GLint lightPositionLoc = glGetUniformLocation(gCubeProgramId, "lightPos");
    GLint viewPositionLoc = glGetUniformLocation(gCubeProgramId, "viewPosition");

    // Pass color, light, and camera data to the Cube Shader program's corresponding uniforms
    glUniform3f(objectColorLoc, gObjectColor.r, gObjectColor.g, gObjectColor.b);
    glUniform3f(lightColorLoc, gLightColor.r, gLightColor.g, gLightColor.b);
    glUniform3f(lightPositionLoc, gLightPosition.x, gLightPosition.y, gLightPosition.z);
    const glm::vec3 cameraPosition = gCamera.Position;
    glUniform3f(viewPositionLoc, cameraPosition.x, cameraPosition.y, cameraPosition.z);

    GLint UVScaleLoc = glGetUniformLocation(gCubeProgramId, "uvScale");
    glUniform2fv(UVScaleLoc, 1, glm::value_ptr(gUVScale)); */

    // Draws the triangles
    glDrawElements(GL_TRIANGLES, gMesh.nIndices, GL_UNSIGNED_SHORT, NULL); // Draws the triangle

    /* Did not work, reverted to previous code
    // LAMP: draw lamp
    //----------------
    glUseProgram(gLampProgramId);

    //Transform the smaller cube used as a visual que for the light source
    model = glm::translate(gLightPosition) * glm::scale(gLightScale);

    // Reference matrix uniforms from the Lamp Shader program
    modelLoc = glGetUniformLocation(gLampProgramId, "model");
    viewLoc = glGetUniformLocation(gLampProgramId, "view");
    projLoc = glGetUniformLocation(gLampProgramId, "projection");

    // Pass matrix data to the Lamp Shader program's matrix uniforms
    glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));
    glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));
    glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));

    glDrawArrays(GL_TRIANGLES, 0, gMesh.nVertices);*/

    // Deactivate the Vertex Array Object
    glBindVertexArray(0);

    // glfw: swap buffers and poll IO events (keys pressed/released, mouse moved etc.)
    glfwSwapBuffers(gWindow);    // Flips the the back buffer with the front buffer every frame.

    // Disables cursor in window to avoid locking camera movement
    glfwSetInputMode(gWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);
}


// Implements the UCreateMesh function
void UCreateMesh(GLMesh& mesh)
{
    // Position and Color data
    GLfloat verts[] = {
        // Vertex Positions    // Colors (r,g,b,a)       // Texture Coordinates
        // Points on top circle
        -0.15f, 0.8f, 0.1f,	      1.0f, 0.0f, 0.0f, 1.0f,  0.25f, 1.0f,   // 0
        -0.185f, 0.8f, 0.085f,    0.0f, 1.0f, 0.0f, 1.0f,  0.2325f, 1.0f, // 1
        -0.2f, 0.8f, 0.05f,       0.0f, 0.0f, 1.0f, 1.0f,  0.215f, 1.0f,  // 2
        -0.185f, 0.8f, 0.015f,	  1.0f, 0.0f, 0.0f, 1.0f,  0.1975f, 1.0f, // 3
        -0.15f, 0.8f, 0.0f,       0.0f, 1.0f, 0.0f, 1.0f,  0.320f, 1.0f,  // 4
        -0.115f, 0.8f, 0.015f,    0.0f, 0.0f, 1.0f, 1.0f,  0.3025f, 1.0f, // 5
        -0.1f, 0.8f, 0.05f,	      1.0f, 0.0f, 0.0f, 1.0f,  0.285f, 1.0f,  // 6
        -0.115f, 0.8f, 0.085f,    0.0f, 1.0f, 0.0f, 1.0f,  0.2675f, 1.0f, // 7

        // Points on bottom circle
        -0.15f, -0.8f, 0.1f,      0.0f, 0.0f, 1.0f, 1.0f,  0.25f, 0.0f,   // 8
        -0.185f, -0.8f, 0.085f,	  1.0f, 0.0f, 0.0f, 1.0f,  0.2325f, 0.0f, // 9
        -0.2f, -0.8f, 0.05f,      0.0f, 1.0f, 0.0f, 1.0f,  0.215f, 0.0f,  // 10
        -0.185f, -0.8f, 0.015f,   0.0f, 0.0f, 1.0f, 1.0f,  0.1975f, 0.0f, // 11
        -0.15f, -0.8f, 0.0f,	  1.0f, 0.0f, 0.0f, 1.0f,  0.320f, 0.0f,  // 12
        -0.115f, -0.8f, 0.015f,   0.0f, 1.0f, 0.0f, 1.0f,  0.3025f, 0.0f, // 13
        -0.1f, -0.8f, 0.05f,      0.0f, 0.0f, 1.0f, 1.0f,  0.285f, 0.0f,  // 14
        -0.115f, -0.8f, 0.085f,	  1.0f, 0.0f, 0.0f, 1.0f,  0.2675f, 0.0f, // 15

        // Middle points in top and bottom circle
        -0.15f, 0.82f, 0.05f,     0.0f, 1.0f, 0.0f, 1.0f,  0.233f, 1.0f,  // 16
        -0.15f, -0.8f, 0.05f,     0.0f, 0.0f, 1.0f, 1.0f,  0.15f, 0.0f,   // 17

        // Plane points
        -0.5f, 1.5f, -0.0001f,    0.5f, 0.0f, 0.5f, 1.0f,  0.5f, 1.0f,    // 18
        -1.7f, -0.5f, -0.0001f,   0.0f, 0.5f, 0.5f, 1.0f,  0.5f, 0.0f,    // 19
        1.3f, -2.3f, -0.0001f,    0.5f, 0.5f, 0.0f, 1.0f,  1.0f, 1.0f,    // 20
        2.5f, -0.3f, -0.0001f,    0.7f, 0.2f, 0.5f, 1.0f,  1.0f, 0.0f,    // 21

        // Box bottom points
        0.8f, 0.5f, 0.0f,         0.0f, 0.0f, 0.0f, 1.0f,  0.1f, 1.0f,    // 22
        -0.1f, -1.0f, 0.0f,       0.0f, 0.0f, 0.0f, 1.0f,  0.1f, 1.0f,    // 23
        0.7f, -1.5f, 0.0f,        0.0f, 0.0f, 0.0f, 1.0f,  0.1f, 1.0f,    // 24
        1.6f, 0.0f, 0.0f,         0.0f, 0.0f, 0.0f, 1.0f,  0.1f, 1.0f,    // 25

        // Box top points
        0.8f, 0.5f, 0.55f,        0.0f, 0.0f, 0.0f, 1.0f,  0.1f, 1.0f,    // 26
        -0.1f, -1.0f, 0.55f,      0.0f, 0.0f, 0.0f, 1.0f,  0.1f, 1.0f,    // 27
        0.7f, -1.5f, 0.55f,       0.0f, 0.0f, 0.0f, 1.0f,  0.1f, 1.0f,    // 28
        1.6f, 0.0f, 0.55f,        0.0f, 0.0f, 0.0f, 1.0f,  0.1f, 1.0f,    // 29

        // Sphere top points
        0.05f, -0.25f, 0.15f,     1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 30
        -0.058f, -0.292f, 0.15f,  1.0f, 1.0f, 1.0f, 1.0f,  0.25f, 1.0f,   // 31
        -0.1f, -0.4f, 0.15f,      1.0f, 1.0f, 1.0f, 1.0f,  0.25f, 1.0f,   // 32
        -0.058f, -0.508f, 0.15f,  1.0f, 1.0f, 1.0f, 1.0f,  0.25f, 1.0f,   // 33
        0.05f, -0.55f, 0.15f,     1.0f, 1.0f, 1.0f, 1.0f,  0.1f, 1.0f,    // 34
        0.158f, -0.508f, 0.15f,   1.0f, 1.0f, 1.0f, 1.0f,  0.1f, 1.0f,    // 35
        0.2f, -0.4f, 0.15f,       1.0f, 1.0f, 1.0f, 1.0f,  0.1f, 1.0f,    // 36
        0.158f, -0.292f, 0.15f,   1.0f, 1.0f, 1.0f, 1.0f,  0.1f, 1.0f,    // 37
        0.05f, -0.292f, 0.258f,   1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 38
        -0.028f, -0.322f, 0.258f, 1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 39
        -0.058f, -0.4f, 0.258f,   1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 40
        -0.028f, -0.478f, 0.258f, 1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 41
        0.05f, -0.508f, 0.258f,   1.0f, 1.0f, 1.0f, 1.0f,  0.1f, 1.0f,    // 42
        0.125f, -0.478f, 0.258f,  1.0f, 1.0f, 1.0f, 1.0f,  0.1f, 1.0f,    // 43
        0.158f, -0.4f, 0.258f,    1.0f, 1.0f, 1.0f, 1.0f,  0.1f, 1.0f,    // 44
        0.128f, -0.322f, 0.258f,  1.0f, 1.0f, 1.0f, 1.0f,  0.1f, 1.0f,    // 45
        0.05f, -0.4f, 0.3f,       1.0f, 1.0f, 1.0f, 1.0f,  0.9f, 1.0f,   // 46

        // Sphere bottom points
        0.05f, -0.292f, 0.042f,   1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 47
        -0.028f, -0.322f, 0.042f, 1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 48
        -0.058f, -0.4f, 0.042f,   1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 49
        -0.028f, -0.478f, 0.042f, 1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 50
        0.05f, -0.508f, 0.042f,   1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 51
        0.128f, -0.478f, 0.042f,  1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 52
        0.158f, -0.4f, 0.042f,    1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 53
        0.128f, -0.322f, 0.042f,  1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f,    // 54
        0.05f, -0.4f, 0.0f,       1.0f, 1.0f, 1.0f, 1.0f,  0.9f, 1.0f    // 55

    };

    // Index data to share position data
    GLushort indices[] = {
        // Cylinder triangles
        0, 1, 8,    // triangle 1
        9, 1, 8,    // triangle 2
        1, 2, 9,    // triangle 3
        10, 2, 9,   // triangle 4
        2, 3, 10,   // triangle 5
        10, 11, 3,  // triangle 6
        3, 4, 11,   // triangle 7
        11, 12, 4,  // triangle 8
        4, 5, 12,   // triangle 9
        12, 13, 5,  // triangle 10
        5, 6, 13,   // triangle 11
        13, 14, 6,  // triangle 12
        6, 7, 14,   // triangle 13
        14, 15, 7,  // triangle 14
        0, 7, 15,   // triangle 15
        15, 8, 0,   // triangle 16

        // Top end triangles
        0, 1, 16,   // triangle 17
        1, 2, 16,   // triangle 18
        2, 3, 16,   // triangle 19
        3, 4, 16,   // triangle 20
        4, 5, 16,   // triangle 21
        5, 6, 16,   // triangle 22
        6, 7, 16,   // triangle 23
        7, 0, 16,   // triangle 24

        // Bottom end triangle
        8, 9, 17,   // triangle 25
        9, 10, 17,  // triangle 26
        10, 11, 17, // triangle 27
        11, 12, 17, // triangle 28
        12, 13, 17, // triangle 29
        13, 14, 17, // triangle 30
        14, 15, 17, // triangle 31
        15, 8, 17,  // triangle 32

        // Plane triangles
        18, 19, 20, // triangle 33
        20, 21, 18, // triangle 34

        // Box traingles
        22, 23, 26, // traingle 35
        23, 26, 27, // traingle 36
        23, 24, 27, // traingle 37
        24, 27, 28, // traingle 38
        24, 25, 28, // traingle 39
        25, 28, 29, // traingle 40
        25, 22, 29, // traingle 41
        22, 29, 26, // traingle 42
        22, 23, 24, // traingle 43
        24, 25, 22, // traingle 44
        26, 27, 28, // traingle 45
        28, 25, 26, // traingle 46

        // Sphere top triangles
        30, 31, 38, // traingle 47
        31, 38, 39, // traingle 48
        31, 32, 40, // traingle 49
        31, 40, 39, // traingle 50
        32, 33, 40, // traingle 51
        33, 40, 41, // traingle 52
        33, 34, 42, // traingle 53
        33, 41, 42, // traingle 54
        34, 35, 42, // traingle 55
        35, 42, 43, // traingle 56
        35, 36, 44, // traingle 57
        35, 43, 44, // traingle 58
        36, 37, 44, // traingle 59
        37, 44, 45, // traingle 60
        37, 38, 45, // traingle 61
        37, 30, 38, // traingle 62
        38, 39, 46, // traingle 63
        39, 40, 46, // traingle 64
        40, 41, 46, // traingle 65
        41, 42, 46, // traingle 66
        42, 43, 46, // traingle 67
        43, 44, 46, // traingle 68
        44, 45, 46, // traingle 69
        45, 38, 46, // traingle 70

        // Sphere bottom triangles
        30, 31, 47, // traingle 71
        31, 47, 48, // traingle 72
        31, 32, 49, // traingle 73
        31, 49, 48, // traingle 74
        32, 33, 49, // traingle 75
        33, 49, 50, // traingle 76
        33, 34, 51, // traingle 77
        33, 50, 51, // traingle 78
        34, 35, 51, // traingle 79
        35, 51, 52, // traingle 80
        35, 36, 53, // traingle 81
        35, 53, 52, // traingle 82
        36, 37, 53, // traingle 83
        37, 53, 54, // traingle 84
        37, 30, 47, // traingle 85
        37, 47, 54, // traingle 86
        47, 48, 55, // traingle 87
        48, 49, 55, // traingle 88
        49, 50, 55, // traingle 89
        50, 51, 55, // traingle 90
        51, 52, 55, // traingle 91
        52, 53, 55, // traingle 92
        53, 54, 55, // traingle 93
        54, 47, 55  // traingle 94

    };

    const GLuint floatsPerVertex = 3;
    const GLuint floatsPerColor = 4;
    const GLuint floatsPerUV = 2;

    glGenVertexArrays(1, &mesh.vao); // we can also generate multiple VAOs or buffers at the same time
    glBindVertexArray(mesh.vao);

    // Create 2 buffers: first one for the vertex data; second one for the indices
    glGenBuffers(2, mesh.vbos);
    glBindBuffer(GL_ARRAY_BUFFER, mesh.vbos[0]); // Activates the buffer
    glBufferData(GL_ARRAY_BUFFER, sizeof(verts), verts, GL_STATIC_DRAW); // Sends vertex or coordinate data to the GPU

    mesh.nIndices = sizeof(indices) / sizeof(indices[0]);
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, mesh.vbos[1]);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

    // Strides between vertex coordinates is 6 (x, y, z, r, g, b, a). A tightly packed stride is 0.
    GLint stride = sizeof(float) * (floatsPerVertex + floatsPerColor + floatsPerUV);// The number of floats before each

    // Create Vertex Attribute Pointers
    glVertexAttribPointer(0, floatsPerVertex, GL_FLOAT, GL_FALSE, stride, 0);
    glEnableVertexAttribArray(0);

    glVertexAttribPointer(1, floatsPerColor, GL_FLOAT, GL_FALSE, stride, (char*)(sizeof(float) * floatsPerVertex));
    glEnableVertexAttribArray(1);

    glVertexAttribPointer(2, floatsPerUV, GL_FLOAT, GL_FALSE, stride, (void*)(sizeof(float) * (floatsPerVertex + floatsPerColor)));
    glEnableVertexAttribArray(2);

    /* tried to switch to just using 
    const GLuint floatsPerVertex = 3;
    const GLuint floatsPerNormal = 3;
    const GLuint floatsPerUV = 2;

    mesh.nVertices = sizeof(verts) / (sizeof(verts[0]) * (floatsPerVertex + floatsPerNormal + floatsPerUV));

    glGenVertexArrays(1, &mesh.vao); // we can also generate multiple VAOs or buffers at the same time
    glBindVertexArray(mesh.vao);

    // Create 2 buffers: first one for the vertex data; second one for the indices
    glGenBuffers(1, &mesh.vbo);
    glBindBuffer(GL_ARRAY_BUFFER, mesh.vbo); // Activates the buffer
    glBufferData(GL_ARRAY_BUFFER, sizeof(verts), verts, GL_STATIC_DRAW); // Sends vertex or coordinate data to the GPU

    // Strides between vertex coordinates is 6 (x, y, z, r, g, b, a). A tightly packed stride is 0.
    GLint stride = sizeof(float) * (floatsPerVertex + floatsPerNormal + floatsPerUV);// The number of floats before each

    // Create Vertex Attribute Pointers
    glVertexAttribPointer(0, floatsPerVertex, GL_FLOAT, GL_FALSE, stride, 0);
    glEnableVertexAttribArray(0);

    glVertexAttribPointer(1, floatsPerNormal, GL_FLOAT, GL_FALSE, stride, (char*)(sizeof(float) * floatsPerVertex));
    glEnableVertexAttribArray(1);

    glVertexAttribPointer(2, floatsPerUV, GL_FLOAT, GL_FALSE, stride, (void*)(sizeof(float) * (floatsPerVertex + floatsPerNormal)));
    glEnableVertexAttribArray(2);*/
}


void UDestroyMesh(GLMesh& mesh)
{
    glDeleteVertexArrays(1, &mesh.vao);
    glDeleteBuffers(2, mesh.vbos);
}

bool UCreateTexture(const char* filename, GLuint& textureId)
{
    int width, height, channels;
    unsigned char* image = stbi_load(filename, &width, &height, &channels, 0);
    if (image)
    {
        flipImageVertically(image, width, height, channels);

        glGenTextures(1, &textureId);
        glBindTexture(GL_TEXTURE_2D, textureId);

        // Set the texture wrapping parameters.
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
        // Set texture filtering parameters.
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

        if (channels == 3)
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB8, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, image);
        else if (channels == 4)
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, image);
        else
        {
            cout << "Not implemented to handle image with " << channels << " channels" << endl;
            return false;
        }

        glGenerateMipmap(GL_TEXTURE_2D);

        stbi_image_free(image);
        glBindTexture(GL_TEXTURE_2D, 0); // Unbind the texture.

        return true;
    }

    // Error loading the image
    return false;
}


// Implements the UCreateShaders function
bool UCreateShaderProgram(const char* vtxShaderSource, const char* fragShaderSource, GLuint& programId)
{
    // Compilation and linkage error reporting
    int success = 0;
    char infoLog[512];

    // Create a Shader program object.
    programId = glCreateProgram();

    // Create the vertex and fragment shader objects
    GLuint vertexShaderId = glCreateShader(GL_VERTEX_SHADER);
    GLuint fragmentShaderId = glCreateShader(GL_FRAGMENT_SHADER);

    // Retrive the shader source
    glShaderSource(vertexShaderId, 1, &vtxShaderSource, NULL);
    glShaderSource(fragmentShaderId, 1, &fragShaderSource, NULL);

    // Compile the vertex shader, and print compilation errors (if any)
    glCompileShader(vertexShaderId); // compile the vertex shader
    // check for shader compile errors
    glGetShaderiv(vertexShaderId, GL_COMPILE_STATUS, &success);
    if (!success)
    {
        glGetShaderInfoLog(vertexShaderId, 512, NULL, infoLog);
        std::cout << "ERROR::SHADER::VERTEX::COMPILATION_FAILED\n" << infoLog << std::endl;

        return false;
    }

    glCompileShader(fragmentShaderId); // compile the fragment shader
    // check for shader compile errors
    glGetShaderiv(fragmentShaderId, GL_COMPILE_STATUS, &success);
    if (!success)
    {
        glGetShaderInfoLog(fragmentShaderId, sizeof(infoLog), NULL, infoLog);
        std::cout << "ERROR::SHADER::FRAGMENT::COMPILATION_FAILED\n" << infoLog << std::endl;

        return false;
    }

    // Attached compiled shaders to the shader program
    glAttachShader(programId, vertexShaderId);
    glAttachShader(programId, fragmentShaderId);

    glLinkProgram(programId);   // links the shader program
    // check for linking errors
    glGetProgramiv(programId, GL_LINK_STATUS, &success);
    if (!success)
    {
        glGetProgramInfoLog(programId, sizeof(infoLog), NULL, infoLog);
        std::cout << "ERROR::SHADER::PROGRAM::LINKING_FAILED\n" << infoLog << std::endl;

        return false;
    }

    glUseProgram(programId);    // Uses the shader program

    return true;
}


void UDestroyShaderProgram(GLuint programId)
{
    glDeleteProgram(programId);
}

